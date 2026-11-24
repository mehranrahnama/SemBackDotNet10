//package com.irisaco.mss.sem.analysisDomain.repository.pestel;
//
//import com.irisaco.mss.sem.analysisDomain.model.dto.pestel.PestelImpactDto;
//import com.irisaco.mss.sem.analysisDomain.model.entity.pestel.PestelImpact;
//import com.irisaco.mss.sem.base.repository.SemRepository;
//import org.springframework.data.domain.Page;
//import org.springframework.data.domain.Pageable;
//import org.springframework.data.jpa.repository.Query;
//import org.springframework.data.repository.query.Param;
//
//import java.util.List;
//
//public interface PestelImpactRepository extends SemRepository<PestelImpact, Long> {
//    @Query("select new com.irisaco.mss.sem.analysisDomain.model.dto.pestel.PestelImpactDto(pi.id,pi.pestel.id,pi.pestel.title, pi.impact, pi.uncertainty) " +
//            "from PestelImpact pi left join pi.pestel p  where ( :filter is null or p.title like %:filter%) and ( :pestelId is null or p.id= :pestelId ) ")
//    Page<PestelImpactDto> findAllPestelImpactPage(Pageable pageable, @Param("pestelId") Long pestelId, @Param("filter") String filter);
//
//    @Query("select new com.irisaco.mss.sem.analysisDomain.model.dto.pestel.PestelImpactDto" +
//            "(pi.id, " +
//            "pi.pestel.id, " +
//            "pi.pestel.title, " +
//            "pi.impact, " +
//            "pi.uncertainty) " +
//            "from PestelImpact pi left join pi.pestel p  " +
//            "where p.pestelGroup.id = :pestelGroupId and  ( :filter is null or p.title like %:filter%)")
//    List<PestelImpactDto> findAllPestelImpactList(Long pestelGroupId, String filter);
//
//}
