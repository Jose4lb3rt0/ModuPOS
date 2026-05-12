package com.jse.modupos.controller;

import com.jse.modupos.dto.CategoriaDTO;
import com.jse.modupos.dto.ProductoDTO;
import com.jse.modupos.service.ProductoService;
import jakarta.validation.Valid;
import lombok.AllArgsConstructor;
import lombok.RequiredArgsConstructor;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;

@RestController
@RequestMapping("/api/productos")
@RequiredArgsConstructor
public class ProductoController {

    private final ProductoService service;

    @GetMapping
    public ResponseEntity<List<ProductoDTO>> listar() {
        return ResponseEntity.ok(service.listarTodas());
    }

    @GetMapping("/{id}")
    public ResponseEntity<ProductoDTO> obtenerPorId(@PathVariable Long id) {
        return ResponseEntity.ok(service.buscarPorId(id));
    }

    @PostMapping(consumes = { MediaType.MULTIPART_FORM_DATA_VALUE })
    public ResponseEntity<ProductoDTO> crear(
            @RequestPart("producto") @Valid ProductoDTO dto,
            @RequestPart(value = "archivo", required = false) MultipartFile archivo
    ) throws IOException {
        return ResponseEntity.ok(service.crear(dto, archivo));
    }

    @PutMapping(value = "/{id}", consumes = { MediaType.MULTIPART_FORM_DATA_VALUE })
    public ResponseEntity<ProductoDTO> actualizar(
            @PathVariable Long id,
            @RequestPart("producto") @Valid ProductoDTO dto,
            @RequestPart(value = "archivo", required = false) MultipartFile archivo
    ) throws IOException {
        return ResponseEntity.ok(service.actualizar(id, dto, archivo));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> eliminar(@PathVariable Long id) {
        service.eliminar(id);
        return ResponseEntity.noContent().build();
    }
}
