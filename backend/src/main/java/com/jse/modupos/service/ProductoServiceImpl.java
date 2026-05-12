package com.jse.modupos.service;

import com.jse.modupos.dto.ProductoDTO;
import com.jse.modupos.mapper.ProductoMapper;
import com.jse.modupos.model.Categoria;
import com.jse.modupos.model.Imagen;
import com.jse.modupos.model.Producto;
import com.jse.modupos.repository.CategoriaRepository;
import com.jse.modupos.repository.ProductoRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.util.List;
import java.util.stream.Collectors;

@Service
@AllArgsConstructor
public class ProductoServiceImpl implements ProductoService {

    private final ProductoRepository productoRepository;
    private final ImagenService imagenService;
    private final ProductoMapper productoMapper;
    private final CategoriaRepository categoriaRepository;

    @Override
    @Transactional(readOnly = true)
    public List<ProductoDTO> listarTodas() {
        return productoRepository.findByIsDeletedFalse()
                .stream()
                .map(productoMapper::toDTO)
                .collect(Collectors.toList());
    }

    @Override
    public ProductoDTO buscarPorId(Long id) {
        return productoMapper.toDTO(productoRepository
                .findById(id)
                .orElseThrow(() -> new RuntimeException("No se encontró el prodcuto con el id: " + id)));
    }

    @Override
    @Transactional
    public ProductoDTO crear(ProductoDTO dto, MultipartFile archivo) throws IOException {
        if (productoRepository.existsByNombreIgnoreCase(dto.getNombre())) {
            throw new RuntimeException("Ya existe el producto con el mismo nombre.");
        }

        if (dto.getSku() != null && productoRepository.existsBySkuIgnoreCase(dto.getSku())) {
            throw new RuntimeException("El sku " + dto.getSku() + " ya existe.");
        }

        Producto producto = productoMapper.toEntity(dto);

        if (archivo != null && !archivo.isEmpty()) {
            Imagen imagenGuardada = imagenService.subirImagen(archivo);
            producto.setImagen(imagenGuardada);
        }

        Categoria categoria = categoriaRepository.findById(dto.getCategoriaId())
                .orElseThrow(() -> new RuntimeException("No existe la categoria."));
        producto.setCategoria(categoria);

        return productoMapper.toDTO(productoRepository.save(producto));
    }

    @Override
    @Transactional
    public ProductoDTO actualizar(Long id, ProductoDTO dto, MultipartFile archivo) throws IOException {
        Producto producto = productoRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("No existe el producto con el id: " + id));

        if (dto.getSku() != null &&
            !dto.getSku().equals(producto.getSku()) &&
            productoRepository.existsBySkuIgnoreCase(dto.getSku())) {
            throw new RuntimeException("El sku " + dto.getSku() + " ya existe.");
        }

        productoMapper.updateEntityFromDto(dto, producto);

        if (!producto.getCategoria().getId().equals(dto.getCategoriaId())) {
            Categoria reemplazo = categoriaRepository.findById(dto.getCategoriaId())
                    .orElseThrow(() -> new RuntimeException("La nueva categoría no existe."));
            producto.setCategoria(reemplazo);
        }

        if (archivo != null && !archivo.isEmpty()) {
            if (producto.getImagen() != null) {
                imagenService.eliminarImagen(producto.getImagen().getId());
            }
            producto.setImagen(imagenService.subirImagen(archivo));
        }

        return productoMapper.toDTO(productoRepository.save(producto));
    }

    @Override
    @Transactional
    public void eliminar(Long id) {
        Producto producto = productoRepository.findById(id).orElseThrow(() -> new RuntimeException("No existe el producto con el id: " + id));
        producto.setIsDeleted(true);
        productoRepository.save(producto);
    }
}
