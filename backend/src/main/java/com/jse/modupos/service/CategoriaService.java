package com.jse.modupos.service;

import com.jse.modupos.dto.CategoriaDTO;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;

public interface CategoriaService {
    List<CategoriaDTO> listarTodas();
    CategoriaDTO buscarPorId(Long id);
    CategoriaDTO crear(CategoriaDTO dto, MultipartFile archivo) throws IOException;
    CategoriaDTO actualizar(Long id, CategoriaDTO dto, MultipartFile archivo) throws IOException;
    void eliminar(Long id);
}
