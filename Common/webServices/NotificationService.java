package com.irisaco.mss.sem.common.webServices;

import com.irisaco.mss.sem.basicDomain.model.entity.PlanPeriod;
import com.irisaco.mss.sem.basicDomain.repository.PlanPeriodRepository;
import com.irisaco.mss.sem.common.logging.SemLogger;
import com.irisaco.mss.sem.common.model.entity.CartableRoyalVision;
import com.irisaco.mss.sem.common.repository.CartableRoyalVisionRepository;
import com.irisaco.mss.sem.securityDomain.model.entity.UserRole;
import com.irisaco.mss.sem.securityDomain.repository.UserRoleRepository;
import org.json.JSONObject;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.*;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClientException;
import org.springframework.web.client.RestTemplate;

import java.util.ArrayList;
import java.util.List;

@Service
public class NotificationService {
    @Autowired
    RestTemplate restTemplate;

    @Autowired
    CartableRoyalVisionRepository cartableRoyalVisionRepository;

    @Autowired
    PlanPeriodRepository planPeriodRepository;

    @Autowired
    UserRoleRepository userRoleRepository;

    @Autowired
    private SemLogger semLogger;


    @Value("${Send.Notification.Url}")
    String sendNotificationUrl;

    public HttpStatus sendNotification(String message, List<String> username) throws Exception {
        semLogger.info("Enter the method");
        HttpHeaders header = new HttpHeaders();
        header.setContentType(MediaType.APPLICATION_JSON);

        JSONObject body = new JSONObject();
        body.put("notifTypeCode", 201);
        body.put("notification", message);
        body.put("username", username);

        HttpEntity<?> httpEntity = new HttpEntity<>(body.toString(), header);
        ResponseEntity<?> response = null;
        try {
            response = restTemplate.exchange(sendNotificationUrl, HttpMethod.POST, httpEntity, String.class, new Object[0]);
            HttpStatus result = HttpStatus.valueOf(response.getStatusCode().value());
            semLogger.info("Exit method");
            return result;
        } catch (RestClientException var7) {
            semLogger.error("throw exception: " + var7.getMessage());
            return HttpStatus.valueOf(response.getStatusCode().value());
        }
    }

    @Scheduled(cron = "0 */30 * * * *", zone = "Asia/Tehran")
    public Boolean sendNotificationScheduler() {
//        semLogger.info("Enter the method");
        fillCartableRoyalVision();

        List<CartableRoyalVision> sendNotificationList = cartableRoyalVisionRepository.findBySmsSent();
        if (!sendNotificationList.isEmpty()) {
            for (CartableRoyalVision item : sendNotificationList) {

                List<String> assignee = new ArrayList<>();
                assignee.add(item.getAssignees().toString());
                try {
                    if (sendNotification("یک رکورد به کارتابل شما اضافه شد.", assignee).equals(HttpStatus.CREATED)) ;
                    {
                        item.setSmsSent(true);
                        cartableRoyalVisionRepository.save(item);
                    }
                } catch (Exception e) {
//                    semLogger.error("throw exception: " + e.getMessage());
                    return true;
                }
            }
        }
//        semLogger.info("Exit method");
        return true;
    }

    void fillCartableRoyalVision() {
//        semLogger.info("Enter the method");
        PlanPeriod planPeriod = planPeriodRepository.findByStatusTypeIsActive();
        List<UserRole> userRole = userRoleRepository.findAdminUserRole();
        Long adminUser = userRole.get(0).getUser().getId();
        List<CartableRoyalVision> dataList = cartableRoyalVisionRepository.getTaskList(planPeriod.getPlanId(), adminUser, "", "id", "DESC");
        for (CartableRoyalVision item : dataList) {
            List<CartableRoyalVision> existed = cartableRoyalVisionRepository.findByTaskurl(item.getTaskurl());
            if (existed.isEmpty()) {
                cartableRoyalVisionRepository.save(item);
            }
        }
//        semLogger.info("Exit method");
    }
}

