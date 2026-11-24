package com.irisaco.mss.sem.common.service.interf;

import com.irisaco.mss.sem.base.model.dto.SearchDto;
import com.irisaco.mss.sem.base.service.inter.BaseService;
import com.irisaco.mss.sem.common.model.dto.AttachmentDto;
import com.irisaco.mss.sem.common.model.entity.Attachment;
import com.irisaco.mss.sem.common.repository.AttachmentRepository;
import org.json.JSONArray;
import org.springframework.core.io.Resource;
import org.springframework.data.domain.Sort;
import org.springframework.web.multipart.MultipartFile;

import java.nio.file.Path;
import java.util.List;

public interface AttachmentService extends BaseService<Attachment, Long, AttachmentRepository> {
    SearchDto loadGridById(String body, int page, int size, String orderBy, SortDirection direction);

    List<AttachmentDto> create(List<MultipartFile> file, String entityName, Long entityId, String des );

    void delete(Long id) throws Exception;

    Attachment findByIdOrElseThrow(Long id);

    Resource loadAsResource(String filename);

    Path load(String filename);

    void createUploadDirectory();

    String createJsonFile(JSONArray jsonArray, String entityName);

    String createAttachment(String fileName, String entityName, Long entityId);
}
