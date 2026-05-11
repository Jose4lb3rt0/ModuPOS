package com.jse.modupos.repository;

import com.jse.modupos.model.Categoria;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface CategoriaRepository extends JpaRepository<Categoria, Long> {
    List<Categoria> findByIsDeletedFalse();

    List<Categoria> findByCategoriaPadreId(Long padreId);

    boolean existsByNombreIgnoreCase(String nombre);
}
