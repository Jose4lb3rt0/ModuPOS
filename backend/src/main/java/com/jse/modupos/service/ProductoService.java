package com.jse.modupos.service;

import com.jse.modupos.dto.ProductoDTO;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;

public interface ProductoService {
    List<ProductoDTO> listarTodas();
    ProductoDTO buscarPorId(Long id);
    ProductoDTO crear(ProductoDTO dto, MultipartFile archivo)  throws IOException;
    ProductoDTO actualizar(Long id, ProductoDTO dto, MultipartFile archivo) throws IOException;
    void eliminar(Long id);
}
