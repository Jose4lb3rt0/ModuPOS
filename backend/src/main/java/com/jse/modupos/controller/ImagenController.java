package com.jse.modupos.controller;

import com.jse.modupos.model.Imagen;
import com.jse.modupos.service.ImagenService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;

@RestController
@RequestMapping("/api/imagenes")
@RequiredArgsConstructor
public class ImagenController {

    private final ImagenService service;

    @PostMapping("/upload")
    public ResponseEntity<Imagen> subir(@RequestParam("file") MultipartFile file) throws IOException {
        return new ResponseEntity<>(service.subirImagen(file), HttpStatus.CREATED);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) throws IOException {
        service.eliminarImagen(id);
        return ResponseEntity.noContent().build();
    }
}
