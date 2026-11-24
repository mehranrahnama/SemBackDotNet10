package com.irisaco.mss.sem.common.webServices;

import com.irisaco.mss.sem.basicDomain.model.entity.MeasureUnit;
import com.irisaco.mss.sem.basicDomain.service.interf.MeasureUnitService;
import com.irisaco.mss.sem.common.logging.SemLogger;
import com.irisaco.mss.sem.measureDomain.model.entity.MeasureCatalog;
import com.irisaco.mss.sem.measureDomain.model.entity.MeasureRepository;
import com.irisaco.mss.sem.measureDomain.service.impl.MeasureCatalogServiceImpl;
import com.irisaco.mss.sem.measureDomain.service.interf.MeasureCatalogService;
import com.irisaco.mss.sem.measureDomain.service.interf.MeasureRepositoryService;
import org.json.JSONArray;
import org.json.JSONObject;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.util.UriComponentsBuilder;

import java.util.HashMap;
import java.util.Map;


@Service
public class HamsooMeasureService {

    @Autowired
    RestTemplate restTemplate;

    @Autowired
    MeasureUnitService measureUnitService;

    @Autowired
    MeasureRepositoryService measureRepositoryService ;

    @Autowired
    MeasureCatalogService measureCatalogService;

    @Autowired
    private MeasureCatalogServiceImpl measureCatalogServiceImpl;

    @Autowired
    private SemLogger semLogger;

    @Value("${hamsoo.user}")
    String hamsooUser;

    @Value("${hamsoo.password}")
    String hamsooPassword;

    @Value("${get.hamsoo.token.url}")
    String getHamsooTokenUrl;

    @Value("${get.hamsoo.measure.url}")
    String getHamsooMeasureUrl;


    public String getHamsooToken() {
        semLogger.info("Enter the method");
        HttpHeaders header = new HttpHeaders();
        header.setContentType(MediaType.APPLICATION_JSON);

        JSONObject body = new JSONObject();
        body.put("username", hamsooUser);
        body.put("password", hamsooPassword);

        HttpEntity<?> httpEntity = new HttpEntity<>(body.toString(), header);
        ResponseEntity<?> response = restTemplate.exchange(getHamsooTokenUrl, HttpMethod.POST, httpEntity, String.class);
        JSONObject jsonObject = new JSONObject(response.getBody().toString());
        String result =  jsonObject.get("token").toString();
        semLogger.info("Exit method");
        return result;

    }


    public void getProcessMeasure(){
        semLogger.info("Enter the method");
        String hamsooToken = getHamsooToken();

        HttpHeaders header = new HttpHeaders();
        header.setContentType(MediaType.APPLICATION_JSON);
        header.setBearerAuth(hamsooToken);
        header.add("curOrgan" , "40");


        String urlTemplate = UriComponentsBuilder.fromHttpUrl(getHamsooMeasureUrl)
                .queryParam("direction" , "{direction}")
                .queryParam("orderBy" , "{orderBy}")
                .queryParam("page" , "{page}")
                .queryParam("size" , "{size}").encode().toUriString();
        Map<String, String> params = new HashMap<>();
        params.put("direction","DESC");
        params.put("orderBy","id");
        params.put("page","0");
        params.put("size","10");


         urlTemplate = getHamsooMeasureUrl.concat("/1402");
//
//
        HttpEntity<?> httpEntity = new HttpEntity<>(header);
//        ResponseEntity<?> response = null;

        ResponseEntity<?> response = restTemplate.exchange(urlTemplate, HttpMethod.GET, httpEntity, String.class, params);
        JSONObject jsonObject = new JSONObject(response.getBody().toString());
        JSONArray content = (JSONArray) jsonObject.get("content");

        insertProcessMeasure(content);
        semLogger.info("Exit method");
    }


    void insertProcessMeasure( JSONArray processMeasures){
        semLogger.info("Enter the method");
        processMeasures.forEach(measureProcess -> {
            // unit
            MeasureUnit measureUnit = measureUnitService.findOrInsertProcessMeasureUnit((JSONObject) ((JSONObject) measureProcess).get("kpiUnit"));
            MeasureRepository measureRepository = measureRepositoryService.findOrInsertProcessMeasureRepository((JSONObject) measureProcess , measureUnit);
            MeasureCatalog measureCatalog = measureCatalogService.findOrInsertMeasureCatalog((JSONObject) measureProcess , measureRepository);
            int a = 0 ;
        });


        // measureRepository
        //measureCatalog

        semLogger.info("Exit method");
    }
}
