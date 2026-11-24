package com.irisaco.mss.sem.analysisDomain.model.entity.pestel;

import com.irisaco.mss.sem.base.model.entity.AuditEntity;
import com.irisaco.mss.sem.basicDomain.model.entity.PlanPeriod;
import jakarta.persistence.*;

@Entity
@Table(name = "tblpestelscore", schema = "analysis")
public class PestelScore extends AuditEntity<Long> {

    private Long id;
    private Pestel pestel;
    private PlanPeriod planPeriod;
    private String opportunityOrThreat;
    private int currentScore;
    private int futureScore;
    private int previousYearCurrentScore;
    private int previousYearFutureScore;


    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    @Column(name = "pestelscoreid", nullable = false)
    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "pestelid", nullable = false, referencedColumnName = "pestelid")
    public Pestel getPestel() {
        return pestel;
    }

    public void setPestel(Pestel pestel) {
        this.pestel = pestel;
    }

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "planid", referencedColumnName = "planid")
    public PlanPeriod getPlanPeriod() {
        return planPeriod;
    }

    public void setPlanPeriod(PlanPeriod planPeriod) {
        this.planPeriod = planPeriod;
    }

    @Column(name = "opportunityorthreat", nullable = false)
    public String getOpportunityOrThreat() {
        return opportunityOrThreat;
    }

    public void setOpportunityOrThreat(String opportunityOrThreat) {
        this.opportunityOrThreat = opportunityOrThreat;
    }

    @Column(name = "currentscore", nullable = false)
    public int getCurrentScore() {
        return currentScore;
    }

    public void setCurrentScore(int currentScore) {
        this.currentScore = currentScore;
    }

    @Column(name = "futurescore", nullable = false)
    public int getFutureScore() {
        return futureScore;
    }

    public void setFutureScore(int futureScore) {
        this.futureScore = futureScore;
    }

    @Column(name = "previousyearcurrentscore", nullable = true)
    public int getPreviousYearCurrentScore() {
        return previousYearCurrentScore;
    }

    public void setPreviousYearCurrentScore(int previousYearCurrentScore) {
        this.previousYearCurrentScore = previousYearCurrentScore;
    }

    @Column(name = "previousyearfuturescore", nullable = true)
    public int getPreviousYearFutureScore() {
        return previousYearFutureScore;
    }

    public void setPreviousYearFutureScore(int previousYearFutureScore) {
        this.previousYearFutureScore = previousYearFutureScore;
    }
}
