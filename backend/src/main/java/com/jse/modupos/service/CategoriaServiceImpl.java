package com.jse.modupos.service;

import com.jse.modupos.dto.CategoriaDTO;
import com.jse.modupos.mapper.CategoriaMapper;
import com.jse.modupos.model.Categoria;
import com.jse.modupos.model.Imagen;
import com.jse.modupos.repository.CategoriaRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class CategoriaServiceImpl implements CategoriaService {

    private final CategoriaRepository categoriaRepository;
    private final CategoriaMapper categoriaMapper;
    private final ImagenService imagenService;

    @Override
    @Transactional(readOnly = true)
    public List<CategoriaDTO> listarTodas() {
        return categoriaRepository.findByIsDeletedFalse()
                .stream()
                .map(categoriaMapper::toDTO)
                .collect(Collectors.toList());
    }

    @Override
    public CategoriaDTO buscarPorId(Long id) {
        return categoriaMapper.toDTO(categoriaRepository
                .findById(id)
                .orElseThrow(() -> new RuntimeException("No se encontro el categoria con id: " + id)));
    }

    @Override
    @Transactional
    public CategoriaDTO crear(CategoriaDTO dto, MultipartFile archivo) throws IOException {
        if (categoriaRepository.existsByNombreIgnoreCase(dto.getNombre())) {
            throw new RuntimeException("Ya existe una categoría con ese nombre.");
        }

        Categoria categoria = categoriaMapper.toEntity(dto);

        if (archivo != null && !archivo.isEmpty()) {
            Imagen imagenGuardada = imagenService.subirImagen(archivo);
            categoria.setImagen(imagenGuardada);
        }

        if (dto.getCategoriaPadreId() != null) {
            Categoria padre = categoriaRepository.findById(dto.getCategoriaPadreId())
                    .orElseThrow(() -> new RuntimeException("No existe el padre"));
            categoria.setCategoriaPadre(padre);
        }

        return categoriaMapper.toDTO(categoriaRepository.save(categoria));
    }

    @Override
    @Transactional
    public CategoriaDTO actualizar(Long id, CategoriaDTO dto, MultipartFile archivo) throws IOException {
        Categoria categoria = categoriaRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("No existe una categoria con el id " + id));

        if (!categoria.getNombre().equalsIgnoreCase(dto.getNombre()) &&
            categoriaRepository.existsByNombreIgnoreCase(dto.getNombre())) {
            throw new RuntimeException("Ya existe otra categoría con el nombre " + dto.getNombre());
        }

        if (dto.getCategoriaPadreId() != null && dto.getCategoriaPadreId().equals(id)) {
            throw new RuntimeException("Una categoría no puede ser padre de sí misma. ");
        }

        /*
        categoria.setNombre(dto.getNombre());
        categoria.setDescripcion(dto.getDescripcion());
        categoria.setColor(dto.getColor());
        */
        categoriaMapper.updateEntityFromDto(dto, categoria);

        if (archivo != null && !archivo.isEmpty()) {
            if (categoria.getImagen() != null) {
                imagenService.eliminarImagen(categoria.getImagen().getId());
            }

            Imagen imagen = imagenService.subirImagen(archivo);
            categoria.setImagen(imagen);
        }

        if (dto.getCategoriaPadreId() != null) {
            Categoria padre = categoriaRepository.findById(dto.getCategoriaPadreId())
                    .orElseThrow(() -> new RuntimeException("Categoría padre no encontrada."));
            categoria.setCategoriaPadre(padre);
        } else {
            categoria.setCategoriaPadre(null);
        }

        return categoriaMapper.toDTO(categoriaRepository.save(categoria));
    }

    @Override
    @Transactional
    public void eliminar(Long id) {
        Categoria categoria = categoriaRepository.findById(id).orElseThrow(() -> new RuntimeException("No se encontró la categoría"));
        categoria.setIsDeleted(true);
        categoriaRepository.save(categoria);
    }
}
