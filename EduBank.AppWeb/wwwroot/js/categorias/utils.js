// wwwroot/js/categorias/utils.js
class CategoriasUtils {
    static token = $('input[name="__RequestVerificationToken"]').val();

    static ajaxOptions(opts) {
        if (this.token) {
            opts.headers = opts.headers || {};
            opts.headers["RequestVerificationToken"] = this.token;
        }
        return opts;
    }

    static mostrarError(xhr) {
        try {
            const json = xhr.responseJSON || JSON.parse(xhr.responseText);
            const msg = json?.mensaje || json?.message || "Error en la solicitud";
            toastr.error(msg);
        } catch (e) {
            toastr.error('Error en la solicitud');
        }
    }

    static iconosDisponibles = [
        // Finanzas
        'bi bi-wallet2',        // Billetera
        'bi bi-cash-coin',      // Moneda efectivo
        'bi bi-currency-exchange', // Cambio de divisas
        'bi bi-graph-up',       // Gráfico subiendo (crecimiento)
        'bi bi-graph-down',     // Gráfico bajando
        'bi bi-piggy-bank',     // Alcancía
        'bi bi-coin',           // Moneda
        'bi bi-cash-stack',     // Pila de efectivo
        'bi bi-bank',           // Banco
        'bi bi-credit-card',    // Tarjeta de crédito
        'bi bi-receipt',        // Recibo
        'bi bi-calculator',     // Calculadora

        // Transporte
        'bi bi-car',            // Coche
        'bi bi-airplane',       // Avión
        'bi bi-bicycle',        // Bicicleta
        'bi bi-bus-front',      // Autobús
        'bi bi-train-freight',  // Tren
        'bi bi-rocket',         // Cohete
        'bi bi-speedometer',    // Velocímetro
        'bi bi-geo',            // Ubicación (para GPS)
        'bi bi-signpost',       // Señal de tráfico
        'bi bi-fuel-pump',      // Surtidor de gasolina

        // Compras
        'bi bi-cart',           // Carrito de compras
        'bi bi-bag',            // Bolsa
        'bi bi-basket',         // Cesta
        'bi bi-basket2',        // Cesta alternativa
        'bi bi-basket3',        // Otra cesta
        'bi bi-tag',            // Etiqueta de precio
        'bi bi-tags',           // Múltiples etiquetas
        'bi bi-receipt',        // Recibo de compra
        'bi bi-gift',           // Regalo
        'bi bi-credit-card',    // Tarjeta de crédito (pago)

        // Comida y Bebidas
        'bi bi-cup-straw',      // Bebida con pajita
        'bi bi-cup',            // Taza
        'bi bi-cup-hot',        // Taza caliente (café/té)
        'bi bi-egg',            // Huevo
        'bi bi-egg-fried',      // Huevo frito
        'bi bi-apple',          // Manzana
        'bi bi-basket',         // Cesta (para la compra)
        'bi bi-utensils',       // Cubiertos
        'bi bi-droplet',        // Gota (líquidos)
        'bi bi-water',          // Agua

        // Casa
        'bi bi-house',          // Casa
        'bi bi-house-door',     // Puerta de casa
        'bi bi-lightbulb',      // Bombilla
        'bi bi-lamp',           // Lámpara
        'bi bi-door-closed',    // Puerta cerrada
        'bi bi-window',         // Ventana
        'bi bi-tree',           // Árbol (jardín)
        'bi bi-flower1',        // Flor
        'bi bi-key',            // Llave
        'bi bi-shield',         // Escudo (seguridad)

        // Salud
        'bi bi-heart',          // Corazón (salud)
        'bi bi-heart-pulse',    // Latido del corazón
        'bi bi-bandaid',        // Tirita
        'bi bi-capsule',        // Cápsula (medicina)
        'bi bi-capsule-pill',   // Píldora
        'bi bi-droplet',        // Gota (sangre/medicina)
        'bi bi-hospital',       // Hospital
        'bi bi-activity',       // Actividad (ritmo cardíaco)

        // Belleza
        'bi bi-droplet',        // Gota (crema/loción)
        'bi bi-brush',          // Brocha de maquillaje
        'bi bi-mirror',         // Espejo
        'bi bi-scissors',       // Tijeras (peluquería)
        'bi bi-gem',            // Gema (joyería)
        'bi bi-eyedropper',     // Cuentagotas (cosméticos)

        // Entretenimiento
        'bi bi-tv',             // Televisor
        'bi bi-laptop',         // Ordenador portátil
        'bi bi-phone',          // Teléfono
        'bi bi-controller',     // Mando de videojuegos
        'bi bi-dice',           // Dado
        'bi bi-dice-1',         // Dado 1
        'bi bi-dice-2',         // Dado 2
        'bi bi-dice-3',         // Dado 3
        'bi bi-dice-4',         // Dado 4
        'bi bi-dice-5',         // Dado 5
        'bi bi-dice-6',         // Dado 6
        'bi bi-music-note',     // Nota musical
        'bi bi-music-note-beamed', // Notas musicales
        'bi bi-film',           // Película
        'bi bi-boombox',        // Radiocasete

        // Cuentas y Contabilidad
        'bi bi-calculator',     // Calculadora
        'bi bi-file-text',      // Documento de texto
        'bi bi-file-spreadsheet', // Hoja de cálculo
        'bi bi-receipt',        // Recibo
        'bi bi-archive',        // Archivo
        'bi bi-folder',         // Carpeta
        'bi bi-folder2',        // Carpeta alternativa
        'bi bi-folder-symlink', // Carpeta con enlace

        // Rutina
        'bi bi-alarm',          // Alarma
        'bi bi-calendar',       // Calendario
        'bi bi-calendar-week',  // Calendario semanal
        'bi bi-calendar-date',  // Fecha
        'bi bi-clock',          // Reloj
        'bi bi-clock-history',  // Reloj history (para tiempos)
        'bi bi-watch',          // Reloj de pulsera

        // Relajación
        'bi bi-emoji-smile',    // Emoticono sonriente
        'bi bi-emoji-sunglasses', // Emoticono con gafas de sol
        'bi bi-flower1',        // Flor
        'bi bi-tree',           // Árbol (naturaleza)
        'bi bi-moon',           // Luna
        'bi bi-cloud',          // Nube
        'bi bi-water',          // Agua
        'bi bi-wind',           // Viento

        // Educación
        'bi bi-book',           // Libro
        'bi bi-journal',        // Revista
        'bi bi-pencil',         // Lápiz
        'bi bi-pen',            // Bolígrafo
        'bi bi-eraser',         // Borrador
        'bi bi-backpack',       // Mochila
        'bi bi-trophy',         // Trofeo
        'bi bi-award',          // Premio
        'bi bi-mortarboard',    // Gorro de graduación

        // Familia
        'bi bi-people',         // Gente
        'bi bi-person',         // Persona
        'bi bi-person-plus',    // Añadir persona
        'bi bi-person-heart',   // Persona con corazón
        'bi bi-heart',          // Corazón (amor familiar)
        'bi bi-house-heart',    // Casa con corazón
        'bi bi-balloon',        // Globo (fiestas)
        'bi bi-balloon-heart',  // Globo con corazón

        // Granja y Mascotas
        'bi bi-heart',          // Corazón (mascotas)
        'bi bi-tree',           // Árbol (granja)
        'bi bi-flower1',        // Flor (jardín)
        'bi bi-egg',            // Huevo (gallinas)
        'bi bi-egg-fried',      // Huevo frito
        'bi bi-shield',         // Escudo (protección)

        // Otros
        'bi bi-star',           // Estrella
        'bi bi-gear',           // Engranaje (configuración)
        'bi bi-tools',          // Herramientas
        'bi bi-envelope',       // Sobre (correo)
        'bi bi-chat',           // Chat
        'bi bi-bell',           // Campana (notificación)
        'bi bi-lightning',      // Rayo
        'bi bi-shield',         // Escudo
        'bi bi-wrench',         // Llave inglesa
        'bi bi-box'             // Caja
    ];

    static coloresDisponibles = [
        // VIOLETAS Y LILAS (1–11)
        { nombre: 'Violeta Oscuro', valor: '#4B0082' },
        { nombre: 'Violeta Profundo', valor: '#6A0DAD' },
        { nombre: 'Púrpura', valor: '#800080' },
        { nombre: 'Amatista', valor: '#9966CC' },
        { nombre: 'Violeta Medio', valor: '#8A2BE2' },
        { nombre: 'Lavanda Oscuro', valor: '#7B68EE' },
        { nombre: 'Orquídea', valor: '#DA70D6' },
        { nombre: 'Orquídea Medio', valor: '#BA55D3' },
        { nombre: 'Violeta Claro', valor: '#D8BFD8' },
        { nombre: 'Lavanda', valor: '#E6E6FA' },
        { nombre: 'Lila', valor: '#C8A2C8' },

        // ROSAS Y FUCSIAS (12–22)
        { nombre: 'Fucsia Oscuro', valor: '#C71585' },
        { nombre: 'Fucsia', valor: '#FF00FF' },
        { nombre: 'Magenta', valor: '#FF00FF' },
        { nombre: 'Rosa Intenso', valor: '#FF1493' },
        { nombre: 'Rosa Frambuesa', valor: '#E30B5D' },
        { nombre: 'Rosa Medio', valor: '#FF69B4' },
        { nombre: 'Rosa Coral', valor: '#F88379' },
        { nombre: 'Rosa Pastel', valor: '#FFB6C1' },
        { nombre: 'Rosa Claro', valor: '#FFD1DC' },
        { nombre: 'Rosa Pálido', valor: '#FADADD' },
        { nombre: 'Rosa Bebé', valor: '#F4C2C2' },

        // ROJOS (23–33)
        { nombre: 'Rojo Oscuro', valor: '#8B0000' },
        { nombre: 'Granate', valor: '#800000' },
        { nombre: 'Rojo Sangre', valor: '#A52A2A' },
        { nombre: 'Rojo Vino', valor: '#800020' },
        { nombre: 'Rojo Fuego', valor: '#B22222' },
        { nombre: 'Rojo', valor: '#FF0000' },
        { nombre: 'Rojo Carmesí', valor: '#DC143C' },
        { nombre: 'Rojo Tomate', valor: '#FF6347' },
        { nombre: 'Rojo Coral', valor: '#FF4040' },
        { nombre: 'Rojo Salmón', valor: '#FA8072' },
        { nombre: 'Rojo Claro', valor: '#FFA07A' },

        // NARANJAS (34–44)
        { nombre: 'Naranja Oscuro', valor: '#FF4500' },
        { nombre: 'Rojo Naranja', valor: '#FF5349' },
        { nombre: 'Naranja', valor: '#FFA500' },
        { nombre: 'Naranja Calabaza', valor: '#FF7518' },
        { nombre: 'Naranja Dorado', valor: '#FFB347' },
        { nombre: 'Melón', valor: '#FDBCB4' },
        { nombre: 'Durazno Oscuro', valor: '#FF9966' },
        { nombre: 'Durazno', valor: '#FFDAB9' },
        { nombre: 'Albaricoque', valor: '#FBCEB1' },
        { nombre: 'Coral Claro', valor: '#F88379' },
        { nombre: 'Naranja Pastel', valor: '#FFD580' },

        // AMARILLOS (45–55)
        { nombre: 'Amarillo Oscuro', valor: '#B8860B' },
        { nombre: 'Amarillo Mostaza', valor: '#FFDB58' },
        { nombre: 'Oro', valor: '#FFD700' },
        { nombre: 'Ámbar', valor: '#FFBF00' },
        { nombre: 'Amarillo', valor: '#FFFF00' },
        { nombre: 'Amarillo Maíz', valor: '#FFF200' },
        { nombre: 'Amarillo Limón', valor: '#FFF44F' },
        { nombre: 'Amarillo Claro', valor: '#FFFF99' },
        { nombre: 'Marfil', valor: '#FFFFF0' },
        { nombre: 'Crema', valor: '#FFFDD0' },
        { nombre: 'Champán', valor: '#F7E7CE' },

        // VERDES (56–66)
        { nombre: 'Verde Oscuro', valor: '#006400' },
        { nombre: 'Verde Bosque', valor: '#228B22' },
        { nombre: 'Verde', valor: '#008000' },
        { nombre: 'Verde Esmeralda', valor: '#50C878' },
        { nombre: 'Verde Lima Oscuro', valor: '#32CD32' },
        { nombre: 'Verde Lima', valor: '#7FFF00' },
        { nombre: 'Verde Pasto', valor: '#7CFC00' },
        { nombre: 'Verde Primavera', valor: '#00FF7F' },
        { nombre: 'Verde Claro', valor: '#90EE90' },
        { nombre: 'Verde Pastel', valor: '#77DD77' },
        { nombre: 'Verde Menta', valor: '#AAF0D1' },

        // CIANES Y TURQUESAS (67–77)
        { nombre: 'Cian Oscuro', valor: '#008B8B' },
        { nombre: 'Turquesa Oscuro', valor: '#00CED1' },
        { nombre: 'Verde Azulado', valor: '#008080' },
        { nombre: 'Cian', valor: '#00FFFF' },
        { nombre: 'Turquesa', valor: '#40E0D0' },
        { nombre: 'Aguamarina', valor: '#7FFFD4' },
        { nombre: 'Celeste Verdoso', valor: '#00FA9A' },
        { nombre: 'Cian Claro', valor: '#E0FFFF' },
        { nombre: 'Celeste Pastel', valor: '#AEEEEE' },
        { nombre: 'Cian Pastel', valor: '#B2FFFF' },
        { nombre: 'Menta Claro', valor: '#AAF0E0' },

        // AZULES (78–88)
        { nombre: 'Azul Marino', valor: '#000080' },
        { nombre: 'Azul Oscuro', valor: '#00008B' },
        { nombre: 'Azul Medianoche', valor: '#191970' },
        { nombre: 'Azul Real', valor: '#4169E1' },
        { nombre: 'Azul', valor: '#0000FF' },
        { nombre: 'Azul Dodger', valor: '#1E90FF' },
        { nombre: 'Azul Acero', valor: '#4682B4' },
        { nombre: 'Azul Celeste', valor: '#87CEEB' },
        { nombre: 'Azul Polvo', valor: '#B0E0E6' },
        { nombre: 'Azul Pastel', valor: '#AEC6CF' },
        { nombre: 'Celeste', valor: '#B2FFFF' },

        // NEUTROS Y GRISES (89–99)
        { nombre: 'Negro', valor: '#000000' },
        { nombre: 'Gris Oscuro', valor: '#2F4F4F' },
        { nombre: 'Gris Pizarra', valor: '#708090' },
        { nombre: 'Gris Medio', valor: '#A9A9A9' },
        { nombre: 'Gris Claro', valor: '#D3D3D3' },
        { nombre: 'Gainsboro', valor: '#DCDCDC' },
        { nombre: 'Plata', valor: '#C0C0C0' },
        { nombre: 'Humo Blanco', valor: '#F5F5F5' },
        { nombre: 'Blanco Antiguo', valor: '#FAEBD7' },
        { nombre: 'Blanco Floral', valor: '#FFFAF0' },
        { nombre: 'Blanco', valor: '#FFFFFF' }
    ];
}