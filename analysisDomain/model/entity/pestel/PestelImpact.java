//package com.irisaco.mss.sem.analysisDomain.model.entity.pestel;
//
//import com.irisaco.mss.sem.analysisDomain.model.entity.enums.Impact;
//import com.irisaco.mss.sem.analysisDomain.model.entity.enums.Uncertainty;
//import com.irisaco.mss.sem.base.model.entity.AuditEntity;
//import jakarta.persistence.*;
//
//@Entity
//@Table(name = "tblpestelimpact",schema = "analysis")
//public class PestelImpact extends AuditEntity<Long> {
//    private Long id;
//    private Pestel pestel;
//    private Impact impact;
//    private Uncertainty uncertainty;
//
//    @Override
//    @Id
//    @GeneratedValue(strategy = GenerationType.IDENTITY)
//    @Column(name = "pestelimpactid", nullable = false)
//    public Long getId() {
//        return id;
//    }
//
//    public void setId(Long id) {
//        this.id = id;
//    }
//    @ManyToOne(fetch = FetchType.LAZY)
//    @JoinColumn(name = "pestelid", nullable = false, referencedColumnName = "pestelid")
//    public Pestel getPestel() {
//        return pestel;
//    }
//
//    public void setPestel(Pestel pestel) {
//        this.pestel = pestel;
//    }
//    @Enumerated(EnumType.ORDINAL)
//    @Column(name = "impact")
//    public Impact getImpact() {
//        return impact;
//    }
//
//    public void setImpact(Impact impact) {
//        this.impact = impact;
//    }
//    @Enumerated(EnumType.ORDINAL)
//    @Column(name = "uncertainty")
//    public Uncertainty getUncertainty() {
//        return uncertainty;
//    }
//
//    public void setUncertainty(Uncertainty uncertainty) {
//        this.uncertainty = uncertainty;
//    }
//}
