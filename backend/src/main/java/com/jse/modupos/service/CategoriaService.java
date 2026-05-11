package com.jse.modupos.service;

import com.jse.modupos.dto.CategoriaDTO;

import java.util.List;

public interface CategoriaService {
    List<CategoriaDTO> listarTodas();
    CategoriaDTO buscarPorId(Long id);
    CategoriaDTO crear(CategoriaDTO dto);
    CategoriaDTO actualizar(Long id, CategoriaDTO dto);
    void eliminar(Long id);
}
