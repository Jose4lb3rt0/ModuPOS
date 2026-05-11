package com.jse.modupos.service;

import com.cloudinary.Cloudinary;
import com.cloudinary.utils.ObjectUtils;
import com.jse.modupos.model.Imagen;
import com.jse.modupos.repository.ImagenRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.Map;

@Service
@AllArgsConstructor
public class ImagenServiceImpl implements ImagenService {

    private final Cloudinary cloudinary;
    private final ImagenRepository imagenRepository;

    @Override
    @Transactional
    public Imagen subirImagen(MultipartFile file) throws IOException {
        Map uploadResult = cloudinary.uploader().upload(file.getBytes(), ObjectUtils.emptyMap());

        String url = (String) uploadResult.get("url");
        String publicId = (String) uploadResult.get("public_id");

        Imagen imagen = new Imagen();
        imagen.setImagenUrl(url);
        imagen.setPublicId(publicId);
        imagen.setNombre(file.getOriginalFilename());
        return imagenRepository.save(imagen);
    }

    @Override
    public void eliminarImagen(Long id) throws IOException {
        Imagen imagen = imagenRepository.findById(id).orElseThrow(() -> new RuntimeException("Imagen no encontrada"));
        cloudinary.uploader().destroy(imagen.getPublicId(), ObjectUtils.emptyMap());
        imagenRepository.delete(imagen);
    }
}
