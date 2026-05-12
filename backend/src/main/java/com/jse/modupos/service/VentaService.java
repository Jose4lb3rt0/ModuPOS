package com.jse.modupos.service;

import com.jse.modupos.dto.VentaDTO;

import java.util.List;

public interface VentaService {
    List<VentaDTO> listarTodas();
    VentaDTO buscarPorId(Long id);
    VentaDTO crear(VentaDTO dto);
    VentaDTO actualizar(Long id, VentaDTO dto);
    void eliminar(Long id);
}
