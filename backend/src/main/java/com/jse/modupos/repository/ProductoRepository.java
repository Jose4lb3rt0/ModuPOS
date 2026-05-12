package com.jse.modupos.repository;

import com.jse.modupos.model.Producto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface ProductoRepository extends JpaRepository<Producto, Long> {
    Optional<Producto> findBySku(String sku);
    List<Producto> findByIsDeletedFalse();
    List<Producto> findByCategoriaIdAndAndIsDeletedFalse(Long categoriaId);
    List<Producto> findByNombreContainingIgnoreCaseAndIsDeletedFalse(String nombre);
    boolean existsBySkuIgnoreCase(String sku);
    boolean existsByNombreIgnoreCase(String nombre);
}
