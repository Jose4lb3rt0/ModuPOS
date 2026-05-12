package com.jse.modupos.controller;

import com.jse.modupos.dto.CategoriaDTO;
import com.jse.modupos.service.CategoriaService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;

@RestController
@RequestMapping("/api/categorias")
@RequiredArgsConstructor
public class CategoriaController {

    private final CategoriaService service;

    @GetMapping
    public ResponseEntity<List<CategoriaDTO>> listar() {
        return ResponseEntity.ok(service.listarTodas());
    }

    @GetMapping("/{id}")
    public ResponseEntity<CategoriaDTO> obtenerPorId(@PathVariable Long id) {
        return ResponseEntity.ok(service.buscarPorId(id));
    }

    @PostMapping(consumes = { MediaType.MULTIPART_FORM_DATA_VALUE })
    public ResponseEntity<CategoriaDTO> crear(
            @RequestPart("categoria") @Valid CategoriaDTO dto,
            @RequestPart(value = "archivo", required = false) MultipartFile archivo
    ) throws IOException {
        return new ResponseEntity<>(service.crear(dto, archivo), HttpStatus.CREATED);
    }

    @PutMapping(value = "/{id}", consumes = { MediaType.MULTIPART_FORM_DATA_VALUE })
    public ResponseEntity<CategoriaDTO> actualizar(
            @PathVariable Long id, 
            @RequestPart("categoria") @Valid CategoriaDTO dto,
            @RequestPart(value = "archivo", required = false) MultipartFile archivo
    ) throws IOException {
        return ResponseEntity.ok(service.actualizar(id, dto, archivo));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<CategoriaDTO> eliminar(@PathVariable Long id) {
        service.eliminar(id);
        return ResponseEntity.noContent().build(); //204
    }
}
