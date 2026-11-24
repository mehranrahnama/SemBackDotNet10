//package com.irisaco.mss.sem.analysisDomain.service.impl.pestel;
//
//import com.irisaco.mss.sem.analysisDomain.model.dto.pestel.PestelEnumsValueDto;
//import com.irisaco.mss.sem.analysisDomain.model.dto.pestel.PestelImpactDto;
//import com.irisaco.mss.sem.analysisDomain.model.entity.pestel.PestelImpact;
//import com.irisaco.mss.sem.analysisDomain.model.entity.enums.Impact;
//import com.irisaco.mss.sem.analysisDomain.model.entity.enums.Uncertainty;
//import com.irisaco.mss.sem.analysisDomain.repository.pestel.PestelImpactRepository;
//import com.irisaco.mss.sem.analysisDomain.service.interf.pestel.PestelImpactService;
//import com.irisaco.mss.sem.analysisDomain.service.interf.pestel.PestelService;
//import com.irisaco.mss.sem.base.model.dto.SearchDto;
//import com.irisaco.mss.sem.Common.exception.BusinessException;
//import com.irisaco.mss.sem.Common.utils.PageUtils;
//import lombok.RequiredArgsConstructor;
//import org.json.JSONObject;
//import org.springframework.data.domain.Page;
//import org.springframework.data.domain.Pageable;
//import org.springframework.data.domain.Sort;
//import org.springframework.stereotype.Service;
//
//import java.util.ArrayList;
//import java.util.Arrays;
//import java.util.List;
//
//@Service
//@RequiredArgsConstructor
//public class PestelImpactServiceImpl implements PestelImpactService {
//    private final PestelImpactRepository pestelImpactRepository;
//    private final PestelService pestelService;
//    @Override
//    public PestelImpactRepository getRepository() {
//        return this.pestelImpactRepository;
//    }
//
//    @Override
//    public PestelImpact findByIdOrElseThrow(Long id) {
//        if (id == null) {
//            throw new BusinessException("error.PestelImpact.idIsNull");
//        }
//        return getRepository().findById(id).orElseThrow(() -> new BusinessException("error.PestelImpact.idIsNotValid"));
//    }
//
//    @Override
//    public List<PestelEnumsValueDto> getAllEnumValues() {
//        List<PestelEnumsValueDto> pestelEnumsValueDtos = new ArrayList<>();
//
//        Arrays.stream(Impact.values()).forEach(impact -> {
//            var pestelEnumValue = new PestelEnumsValueDto();
//            pestelEnumValue.setOrdinalValue(impact.ordinal());
//            pestelEnumValue.setStringValue(impact.name());
//            pestelEnumValue.setEnumsType(impact.getDeclaringClass().getSimpleName());
//            pestelEnumsValueDtos.add(pestelEnumValue);
//        });
//        Arrays.stream(Uncertainty.values()).forEach(uncertainty -> {
//            var pestelEnumValue = new PestelEnumsValueDto();
//            pestelEnumValue.setOrdinalValue(uncertainty.ordinal());
//            pestelEnumValue.setStringValue(uncertainty.name());
//            pestelEnumValue.setEnumsType(uncertainty.getDeclaringClass().getSimpleName());
//            pestelEnumsValueDtos.add(pestelEnumValue);
//        });
//        return pestelEnumsValueDtos;
//    }
//
//    @Override
//    public void createNewPestelImpact(List<PestelImpactDto> dtoList) {
//        List<PestelImpact> transientEntities = new ArrayList<>();
//        dtoList.stream().map(this::mapToEntity).forEach(transientEntities::add);
//        pestelImpactRepository.saveAll(transientEntities);
//    }
//
//    @Override
//    public PestelImpactDto getPestelImpactById(Long id) {
//        return mapToDto(findByIdOrElseThrow(id));
//    }
//
//    @Override
//    public void editPestelImpactService(List<PestelImpactDto> dtoList) {
//        List<PestelImpact> persistedEntities = new ArrayList<>();
//        dtoList.forEach(dto->{
//            var persistedEntity = findByIdOrElseThrow(dto.getId());
//            if (dto.getUncertainty()!=null)
//                persistedEntity.setUncertainty(dto.getUncertainty());
//            if (dto.getImpact()!=null)
//                persistedEntity.setImpact(dto.getImpact());
//            if (dto.getPestelId()!=null && !dto.getPestelId().equals(persistedEntity.getPestel().getId())) {
//                var pestel = pestelService.findByIdOrElseThrow(dto.getPestelId());
//                persistedEntity.setPestel(pestel);
//            }
//            persistedEntities.add(persistedEntity);
//        });
//        pestelImpactRepository.saveAll(persistedEntities);
//    }
//
//    @Override
//    public void deletePestelImpacts(List<PestelImpactDto> dtoList) {
//        List<PestelImpact> deletingEntities = dtoList.stream().map(dto -> findByIdOrElseThrow(dto.getId())).toList();
//        pestelImpactRepository.deleteAll(deletingEntities);
//    }
//
//    @Override
//    public SearchDto getAllPestelImpactsPage(int page, int size, String orderBy, SortDirection direction, String filter, Long pestelId) {
//        Pageable pageable = PageUtils.getInstance(page,size,orderBy,direction);
//        Page<PestelImpactDto> res = pestelImpactRepository.findAllPestelImpactPage(pageable,pestelId,filter);
//        return new SearchDto.Builder()
//                .content(res.getContent())
//                .currentPage(page)
//                .pageSize(size)
//                .totalElements(res.getTotalElements())
//                .totalPages(res.getTotalPages())
//                .build();
//    }
//
//    @Override
//    public List<PestelImpactDto> getAllPestelImpactsList(String body) {
//        JSONObject jsonObject = new JSONObject(body);
//        String filter = !jsonObject.isNull("filter") ? jsonObject.getString("filter") : null;
//        Long pestelGroupId = !jsonObject.isNull("pestelGroupId") ? jsonObject.getLong("pestelGroupId") : null;
//
//        List<PestelImpactDto> res = pestelImpactRepository.findAllPestelImpactList(pestelGroupId, filter);
//        if (res.isEmpty())
//            return null;
//        return res;
//    }
//
//    private PestelImpactDto mapToDto(PestelImpact pestelImpact) {
//        return new PestelImpactDto(pestelImpact.getId(),pestelImpact.getPestel().getId(),pestelImpact.getPestel().getTitle(),pestelImpact.getImpact(),pestelImpact.getUncertainty());
//    }
//
//    private PestelImpact mapToEntity(PestelImpactDto dto){
//        PestelImpact pestelImpact = new PestelImpact();
//        var pestel = pestelService.findByIdOrElseThrow(dto.getPestelId());
//        pestelImpact.setPestel(pestel);
//        pestelImpact.setImpact(dto.getImpact());
//        pestelImpact.setUncertainty(dto.getUncertainty());
//        return pestelImpact;
//    }
//}
