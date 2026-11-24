using System;
using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SemBackDotNet10.actionDomain.persistence;

/// <summary>
/// One scoped context per planning workflow. SaveChanges calls made by nested
/// repositories flush but cannot commit the enclosing database transaction.
/// A failed instance must not be reused, including after a caller-owned rollback.
/// </summary>
public sealed class ActionPlanningUnitOfWork(DbContext db)
{
    private bool _failed;
    private bool _executing;

    public bool UsesContext(DbContext context) => ReferenceEquals(db, context);

    public void Execute(Action operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (_failed)
            throw new InvalidOperationException("The failed planning scope must be disposed.");
        if (_executing)
            throw new InvalidOperationException("Recursive planning workflows are not supported.");

        var ownsTransaction = db.Database.CurrentTransaction is null;
        _executing = true;
        try
        {
            if (ownsTransaction)
            {
                // EF requires an execution-strategy scope around explicit transactions.
                // Wrap failures to prevent replaying this stateful, scoped workflow.
                try
                {
                    db.Database.CreateExecutionStrategy().Execute(() =>
                    {
                        try { ExecuteOnce(operation, true); }
                        catch (Exception error) { throw new NonReplayableWorkflowException(error); }
                    });
                }
                catch (NonReplayableWorkflowException error)
                {
                    ExceptionDispatchInfo.Capture(error.InnerException!).Throw();
                    throw;
                }
            }
            else ExecuteOnce(operation, false);
        }
        catch
        {
            _failed = true;
            throw;
        }
        finally { _executing = false; }
    }

    private void ExecuteOnce(Action operation, bool ownsTransaction)
    {
        IDbContextTransaction? transaction = null;
        try
        {
            if (ownsTransaction) transaction = db.Database.BeginTransaction();
            operation();
            db.SaveChanges();
            transaction?.Commit();
        }
        catch (Exception original)
        {
            if (transaction is not null)
            {
                try { transaction.Rollback(); }
                catch (Exception rollbackError) { original.Data["ActionPlanning.RollbackError"] = rollbackError; }
            }
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private sealed class NonReplayableWorkflowException(Exception inner) : Exception("The workflow must not be replayed.", inner);
}
