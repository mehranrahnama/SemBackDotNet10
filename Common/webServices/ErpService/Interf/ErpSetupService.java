package com.irisaco.mss.sem.common.webServices.ErpService.Interf;

import org.springframework.http.ResponseEntity;

import java.util.List;

public interface ErpSetupService {

    String getToken();
    ResponseEntity<?> callErpSmsService(String message, List<String> numbers);
}
