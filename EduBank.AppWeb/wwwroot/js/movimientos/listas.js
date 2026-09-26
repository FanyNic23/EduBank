// wwwroot/js/movimientos/listas.js
class ListasManager {
    constructor(manager) {
        this.manager = manager;
    }

    inicializar() {
        this.configurarEventosLista();
    }

    configurarEventosLista() {
        // Eventos para filtros de lista
        $('#filtroTipo').on('change', () => this.actualizarListaMovimientos());
        $('#filtroCategoria').on('change', () => this.actualizarListaMovimientos());
        $('#filtroFecha').on('change', () => this.actualizarListaMovimientos());
    }

    // Cargar movimientos recientes
    async cargarMovimientosRecientes() {
        try {
            const response = await $.ajax({
                url: '/Movimiento/Lista',
                method: 'GET',
                dataType: 'json'
            });

            if (response.valor && response.movimientos) {
                this.mostrarMovimientosEnLista(response.movimientos);
            }
        } catch (error) {
            console.error('Error cargando movimientos:', error);
        }
    }

    // Actualizar lista de movimientos
    async actualizarListaMovimientos() {
        await this.cargarMovimientosRecientes();
    }

    // Mostrar movimientos en la lista
    mostrarMovimientosEnLista(movimientos) {
        const contenedor = $('#listaMovimientos');
        contenedor.empty();

        if (!movimientos || movimientos.length === 0) {
            contenedor.append(`
                <div class="text-center text-muted py-4">
                    <i class="fas fa-receipt fa-2x mb-2"></i>
                    <p>No hay movimientos registrados</p>
                </div>
            `);
            return;
        }

        // Ordenar por fecha más reciente primero
        movimientos.sort((a, b) => new Date(b.FechaOperacion) - new Date(a.FechaOperacion));

        movimientos.forEach(mov => {
            const elemento = this.crearElementoMovimiento(mov);
            contenedor.append(elemento);
        });
    }

    // Crear elemento HTML para un movimiento
    crearElementoMovimiento(movimiento) {
        const esGasto = MovimientosUtils.isGasto(movimiento.Tipo);
        const icono = movimiento.CategoriaIcono || (esGasto ? 'fa-arrow-down' : 'fa-arrow-up');
        const claseColor = esGasto ? 'text-danger' : 'text-success';
        const signo = esGasto ? '-' : '+';
        const fecha = new Date(movimiento.FechaOperacion);
        const fechaFormateada = fecha.toLocaleDateString('es-ES', {
            day: 'numeric',
            month: 'short',
            year: 'numeric'
        });

        return `
            <div class="list-group-item movimiento-item" data-movimiento-id="${movimiento.MovimientoId}">
                <div class="d-flex justify-content-between align-items-center">
                    <div class="d-flex align-items-center">
                        <div class="me-3 ${claseColor}">
                            <i class="fas ${icono} fa-lg"></i>
                        </div>
                        <div>
                            <h6 class="mb-0">${movimiento.CategoriaNombre || 'Sin categoría'}</h6>
                            <small class="text-muted">${movimiento.Comentario || 'Sin comentario'}</small>
                            <br>
                            <small class="text-muted">${fechaFormateada}</small>
                        </div>
                    </div>
                    <div class="text-end">
                        <div class="${claseColor} fw-bold fs-5">
                            ${signo} ${MovimientosUtils.fmt(Math.abs(movimiento.Monto))}
                        </div>
                        <small class="text-muted">${this.manager.obtenerNombreCuenta(movimiento.CuentaId)}</small>
                        <div class="mt-1">
                            <button class="btn btn-sm btn-outline-primary editar-movimiento" data-id="${movimiento.MovimientoId}">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger eliminar-movimiento" data-id="${movimiento.MovimientoId}">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
}