//package com.irisaco.mss.sem.analysisDomain.model.entity.stakeHolder;
//
//import com.irisaco.mss.sem.analysisDomain.model.entity.enums.BehaviorStrategy;
//import com.irisaco.mss.sem.base.model.entity.AuditEntity;
//import jakarta.persistence.*;
//
//@Entity
//@Table(name = "tblstakeholderpriority", schema = "analysis")
//public class StakeHolderPriority extends AuditEntity<Long> {
//
//    private Long id;
//    private StakeHolder stakeHolder;
//    private Long power;
//    private Long interest;
//    private BehaviorStrategy behaviorStrategy;
//
//
//    @GeneratedValue(strategy = GenerationType.IDENTITY)
//    @Id
//    @Column(name = "stakeholderpriorityid", nullable = false)
//    public Long getId() {
//        return id;
//    }
//
//    public void setId(Long id) {
//        this.id = id;
//    }
//
//    @ManyToOne(fetch = FetchType.LAZY)
//    @JoinColumn(name = "stakeholderid")
//    public StakeHolder getStakeHolder() {
//        return stakeHolder;
//    }
//
//    public void setStakeHolder(StakeHolder stakeHolder) {
//        this.stakeHolder = stakeHolder;
//    }
//
//    @Column(name = "Power", nullable = false)
//    public Long getPower() {
//        return power;
//    }
//
//    public void setPower(Long power) {
//        this.power = power;
//    }
//
//    @Column(name = "Interest", nullable = false)
//    public Long getInterest() {
//        return interest;
//    }
//
//    public void setInterest(Long interest) {
//        this.interest = interest;
//    }
//
//    @Column(name = "behaviorstrategy")
//    @Enumerated(EnumType.STRING)
//    public BehaviorStrategy getBehaviorStrategy() {
//        return behaviorStrategy;
//    }
//
//    public void setBehaviorStrategy(BehaviorStrategy behaviorStrategy) {
//        this.behaviorStrategy = behaviorStrategy;
//    }
//}
