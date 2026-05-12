package com.jse.modupos.mapper;

import com.jse.modupos.dto.DetalleVentaDTO;
import com.jse.modupos.dto.VentaDTO;
import com.jse.modupos.model.Venta;
import com.jse.modupos.model.VentaDetalle;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValuePropertyMappingStrategy;

@Mapper(componentModel = "spring", nullValuePropertyMappingStrategy = NullValuePropertyMappingStrategy.IGNORE)
public interface VentaMapper {

    @Mapping(source = "impuestos", target = "impuesto")
    @Mapping(source = "metodoPago.id", target = "metodoPagoId")
    @Mapping(source = "metodoPago.nombre", target = "metodoPagoNombre")
    VentaDTO toDTO(Venta venta);

    @Mapping(target = "metodoPago", ignore = true)
    @Mapping(target = "detalles", ignore = true)
    @Mapping(target = "subtotal", ignore = true)
    @Mapping(target = "impuestos", ignore = true)
    @Mapping(target = "total", ignore = true)
    @Mapping(target = "estado", ignore = true)
    @Mapping(target = "folio", ignore = true)
    @Mapping(target = "fecha", ignore = true)
    @Mapping(target = "descuentoTotal", ignore = true)
    Venta toEntity(VentaDTO dto);

    @Mapping(source = "venta.id", target = "ventaId")
    @Mapping(source = "producto.id", target = "productoId")
    @Mapping(source = "producto.nombre", target = "productoNombre")
    DetalleVentaDTO toDetalleDTO(VentaDetalle detalle);

    @Mapping(target = "venta", ignore = true)
    @Mapping(target = "producto", ignore = true)
    VentaDetalle toDetalleEntity(DetalleVentaDTO dto);

    @Mapping(target = "id", ignore = true)
    @Mapping(target = "metodoPago", ignore = true)
    @Mapping(target = "detalles", ignore = true)
    @Mapping(target = "subtotal", ignore = true)
    @Mapping(target = "impuestos", ignore = true)
    @Mapping(target = "total", ignore = true)
    @Mapping(target = "estado", ignore = true)
    @Mapping(target = "folio", ignore = true)
    @Mapping(target = "fecha", ignore = true)
    @Mapping(target = "descuentoTotal", ignore = true)
    @Mapping(target = "isDeleted", ignore = true)
    void updateEntityFromDto(VentaDTO dto, @MappingTarget Venta venta);
}
