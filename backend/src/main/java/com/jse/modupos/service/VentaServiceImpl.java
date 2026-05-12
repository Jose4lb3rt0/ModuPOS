package com.jse.modupos.service;

import com.jse.modupos.dto.DetalleVentaDTO;
import com.jse.modupos.dto.VentaDTO;
import com.jse.modupos.mapper.VentaMapper;
import com.jse.modupos.model.EstadoVenta;
import com.jse.modupos.model.MetodoPago;
import com.jse.modupos.model.Producto;
import com.jse.modupos.model.Venta;
import com.jse.modupos.model.VentaDetalle;
import com.jse.modupos.repository.MetodoPagoRepository;
import com.jse.modupos.repository.ProductoRepository;
import com.jse.modupos.repository.VentaRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class VentaServiceImpl implements VentaService {

    private static final DateTimeFormatter FOLIO_FORMAT = DateTimeFormatter.ofPattern("yyyyMMddHHmmss");

    private final VentaRepository ventaRepository;
    private final ProductoRepository productoRepository;
    private final MetodoPagoRepository metodoPagoRepository;
    private final VentaMapper ventaMapper;

    @Override
    @Transactional(readOnly = true)
    public List<VentaDTO> listarTodas() {
        return ventaRepository.findByIsDeletedFalse()
                .stream()
                .map(ventaMapper::toDTO)
                .collect(Collectors.toList());
    }

    @Override
    @Transactional(readOnly = true)
    public VentaDTO buscarPorId(Long id) {
        return ventaMapper.toDTO(obtenerVentaActiva(id));
    }

    @Override
    @Transactional
    public VentaDTO crear(VentaDTO dto) {
        MetodoPago metodoPago = metodoPagoRepository.findById(dto.getMetodoPagoId())
                .orElseThrow(() -> new RuntimeException("No existe el metodo de pago con el id: " + dto.getMetodoPagoId()));

        Venta venta = ventaMapper.toEntity(dto);
        venta.setMetodoPago(metodoPago);
        venta.setEstado(EstadoVenta.COMPLETADA);
        venta.setFecha(LocalDateTime.now());
        venta.setFolio(generarFolio());
        reconstruirDetallesYTotales(venta, dto);

        return ventaMapper.toDTO(ventaRepository.save(venta));
    }

    @Override
    @Transactional
    public VentaDTO actualizar(Long id, VentaDTO dto) {
        Venta venta = obtenerVentaActiva(id);
        MetodoPago metodoPago = metodoPagoRepository.findById(dto.getMetodoPagoId())
                .orElseThrow(() -> new RuntimeException("No existe el metodo de pago con el id: " + dto.getMetodoPagoId()));

        restaurarStock(venta);
        ventaMapper.updateEntityFromDto(dto, venta);
        venta.setMetodoPago(metodoPago);
        reconstruirDetallesYTotales(venta, dto);

        return ventaMapper.toDTO(ventaRepository.save(venta));
    }

    @Override
    @Transactional
    public void eliminar(Long id) {
        Venta venta = obtenerVentaActiva(id);
        venta.setIsDeleted(true);
        ventaRepository.save(venta);
    }

    private Venta obtenerVentaActiva(Long id) {
        Venta venta = ventaRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("No existe la venta con el id: " + id));

        if (Boolean.TRUE.equals(venta.getIsDeleted())) {
            throw new RuntimeException("La venta con el id " + id + " fue eliminada.");
        }

        return venta;
    }

    private void validarStockDisponible(Producto producto, Integer cantidadSolicitada) {
        if (cantidadSolicitada == null || cantidadSolicitada <= 0) {
            throw new RuntimeException("La cantidad solicitada para el producto " + producto.getNombre() + " debe ser mayor a 0.");
        }

        if (producto.getStock() < cantidadSolicitada) {
            throw new RuntimeException(
                    "Stock insuficiente para el producto " + producto.getNombre() +
                            ". Stock disponible: " + producto.getStock() +
                            ", cantidad solicitada: " + cantidadSolicitada
            );
        }
    }

    private void restaurarStock(Venta venta) {
        for (VentaDetalle detalle : venta.getDetalles()) {
            Producto producto = detalle.getProducto();
            producto.setStock(producto.getStock() + detalle.getCantidad());
        }
    }

    private void reconstruirDetallesYTotales(Venta venta, VentaDTO dto) {
        venta.clearDetalles();
        BigDecimal subtotalVenta = BigDecimal.ZERO;

        for (DetalleVentaDTO detalleDTO : dto.getDetalles()) {
            Producto producto = productoRepository.findById(detalleDTO.getProductoId())
                    .orElseThrow(() -> new RuntimeException("No existe el producto con el id: " + detalleDTO.getProductoId()));

            validarStockDisponible(producto, detalleDTO.getCantidad());

            BigDecimal precioUnitario = producto.getPrecioActual();
            BigDecimal subtotalDetalle = precioUnitario.multiply(BigDecimal.valueOf(detalleDTO.getCantidad()));

            VentaDetalle detalle = ventaMapper.toDetalleEntity(detalleDTO);
            detalle.setProducto(producto);
            detalle.setCantidad(detalleDTO.getCantidad());
            detalle.setPrecioUnitario(precioUnitario);
            detalle.setSubtotal(subtotalDetalle);
            venta.addDetalle(detalle);

            producto.setStock(producto.getStock() - detalleDTO.getCantidad());
            subtotalVenta = subtotalVenta.add(subtotalDetalle);
        }

        BigDecimal impuesto = dto.getImpuesto() != null ? dto.getImpuesto() : BigDecimal.ZERO;
        venta.setSubtotal(subtotalVenta);
        venta.setImpuestos(impuesto);
        venta.setTotal(subtotalVenta.add(impuesto));
    }

    private String generarFolio() {
        long sufijo = Math.abs(System.currentTimeMillis() % 10_000);
        return "V-" + LocalDateTime.now().format(FOLIO_FORMAT) + String.format("%04d", sufijo);
    }
}
