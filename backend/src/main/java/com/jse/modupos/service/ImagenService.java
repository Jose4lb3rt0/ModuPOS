package com.jse.modupos.service;

import com.jse.modupos.model.Imagen;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;

public interface ImagenService {
    Imagen subirImagen(MultipartFile file) throws IOException;
    void eliminarImagen(Long id) throws IOException;
}
