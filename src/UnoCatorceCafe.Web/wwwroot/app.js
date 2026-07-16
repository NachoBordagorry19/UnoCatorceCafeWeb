// Estado Global de la Aplicación
let menuData = [];
let selectedCategoryId = null;
let cart = [];

// Elementos del DOM
const contenedorCategorias = document.getElementById('contenedor-categorias');
const contenedorProductos = document.getElementById('contenedor-productos');
const btnVerCarrito = document.getElementById('btn-ver-carrito');
const panelCarrito = document.getElementById('panel-carrito');
const overlayCarrito = document.getElementById('overlay-carrito');
const btnCerrarCarrito = document.getElementById('btn-cerrar-carrito');
const listaCarritoItems = document.getElementById('lista-carrito-items');
const resumenValoresCarrito = document.getElementById('resumen-valores-carrito');
const cartSubtotal = document.getElementById('cart-subtotal');
const cartTotal = document.getElementById('cart-total');
const cartBadgeCount = document.getElementById('cart-badge-count');
const btnIniciarCheckout = document.getElementById('btn-iniciar-checkout');

const modalCheckout = document.getElementById('modal-checkout');
const btnCerrarModal = document.getElementById('btn-cerrar-modal');
const formularioPedido = document.getElementById('formulario-pedido');
const campoTipoEntrega = document.getElementById('campo-tipo-entrega');
const grupoDireccion = document.getElementById('grupo-direccion');
const campoDireccion = document.getElementById('campo-direccion');

const toastNotificacion = document.getElementById('toast-notificacion');
const toastMensaje = document.getElementById('toast-mensaje');

// Cargar Datos Iniciales al iniciar la página
document.addEventListener('DOMContentLoaded', () => {
    inicializarApp();
});

// Inicializar la aplicación
async function inicializarApp() {
    configurarEventos();
    await cargarMenu();
}

// Configurar todos los listeners de eventos
function configurarEventos() {
    // Abrir y cerrar Carrito
    btnVerCarrito.addEventListener('click', abrirCarrito);
    btnCerrarCarrito.addEventListener('click', cerrarCarrito);
    overlayCarrito.addEventListener('click', cerrarCarrito);

    // Abrir y cerrar Modal de Checkout
    btnIniciarCheckout.addEventListener('click', abrirCheckout);
    btnCerrarModal.addEventListener('click', cerrarCheckout);
    
    // Control condicional del campo Dirección de Delivery
    campoTipoEntrega.addEventListener('change', (e) => {
        if (e.target.value === 'Delivery') {
            grupoDireccion.style.display = 'block';
            campoDireccion.required = true;
        } else {
            grupoDireccion.style.display = 'none';
            campoDireccion.required = false;
            campoDireccion.value = '';
        }
    });

    // Enviar formulario (WhatsApp)
    formularioPedido.addEventListener('submit', procesarEnvioPedido);
}

// Consumir API del Backend para obtener el Menú
async function cargarMenu() {
    try {
        const respuesta = await fetch('/api/menu');
        if (!respuesta.ok) {
            throw new Error('No se pudo obtener el menú');
        }
        
        menuData = await respuesta.json();
        
        if (menuData.length > 0) {
            // Seleccionar la primera categoría por defecto
            selectedCategoryId = menuData[0].Id || menuData[0].id;
            renderizarCategorias();
            renderizarProductos();
        } else {
            contenedorProductos.innerHTML = '<p class="empty-cart-message">No hay productos activos en este momento.</p>';
        }
    } catch (error) {
        console.error('Error cargando el menú:', error);
        contenedorCategorias.innerHTML = '';
        contenedorProductos.innerHTML = `
            <div class="empty-cart-message">
                <i class="fa-solid fa-triangle-exclamation" style="font-size: 2rem; color: var(--accent-gold); margin-bottom: 1rem;"></i>
                <p>Ocurrió un error al cargar el menú dinámico.</p>
                <p style="font-size: 0.8rem; margin-top: 0.5rem; color: var(--text-muted);">Verificá la conexión al servidor de base de datos.</p>
            </div>`;
    }
}

// Renderizar Pestañas de Categorías
function renderizarCategorias() {
    contenedorCategorias.innerHTML = '';
    
    menuData.forEach(cat => {
        const idCat = cat.Id || cat.id;
        const nombreCat = cat.Nombre || cat.nombre;
        
        const tab = document.createElement('button');
        tab.className = `category-tab ${idCat === selectedCategoryId ? 'active' : ''}`;
        tab.textContent = nombreCat;
        tab.dataset.id = idCat;
        
        tab.addEventListener('click', () => {
            selectedCategoryId = idCat;
            
            // Actualizar pestañas activas
            document.querySelectorAll('.category-tab').forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
            
            renderizarProductos();
        });
        
        contenedorCategorias.appendChild(tab);
    });
}

// Renderizar Productos de la Categoría Seleccionada
function renderizarProductos() {
    contenedorProductos.innerHTML = '';
    
    const categoriaActiva = menuData.find(cat => (cat.Id || cat.id) === selectedCategoryId);
    if (!categoriaActiva) return;
    
    const productos = categoriaActiva.Productos || categoriaActiva.productos || [];
    
    if (productos.length === 0) {
        contenedorProductos.innerHTML = '<p class="empty-cart-message">No hay productos en esta categoría.</p>';
        return;
    }
    
    productos.forEach(prod => {
        const idProd = prod.Id || prod.id;
        const nombreProd = prod.Nombre || prod.nombre;
        const descProd = prod.Descripcion || prod.descripcion;
        const precioProd = prod.Precio || prod.precio;
        const imgProd = prod.UrlImagen || prod.urlImagen || '/images/latte_art_hero.jpg';
        
        const card = document.createElement('article');
        card.className = 'product-card animate-fade-in';
        
        card.innerHTML = `
            <div class="product-img-wrapper">
                <img src="${imgProd}" alt="${nombreProd}" onerror="this.src='/images/latte_art_hero.jpg'">
            </div>
            <div class="product-info">
                <h3 class="product-name">${nombreProd}</h3>
                <p class="product-desc">${descProd}</p>
                <div class="product-footer">
                    <span class="product-price">$${precioProd}</span>
                    <button class="btn-add-cart" data-id="${idProd}" aria-label="Agregar ${nombreProd} al carrito">
                        <i class="fa-solid fa-plus"></i>
                    </button>
                </div>
            </div>
        `;
        
        // Agregar manejador al botón de agregar al carrito
        card.querySelector('.btn-add-cart').addEventListener('click', () => {
            agregarAlCarrito(prod);
        });
        
        contenedorProductos.appendChild(card);
    });
}

// Agregar un ítem al carrito de compras
function agregarAlCarrito(producto) {
    const idProd = producto.Id || producto.id;
    const nombreProd = producto.Nombre || producto.nombre;
    const precioProd = producto.Precio || producto.precio;

    const itemExistente = cart.find(item => item.id === idProd);
    
    if (itemExistente) {
        itemExistente.cantidad++;
    } else {
        cart.push({
            id: idProd,
            nombre: nombreProd,
            precio: precioProd,
            cantidad: 1
        });
    }
    
    actualizarCarrito();
    mostrarToast(`Agregado: ${nombreProd}`);
}

// Actualizar cantidad de un producto
function modificarCantidad(idProducto, delta) {
    const item = cart.find(item => item.id === idProducto);
    if (!item) return;
    
    item.cantidad += delta;
    
    if (item.cantidad <= 0) {
        cart = cart.filter(item => item.id !== idProducto);
    }
    
    actualizarCarrito();
}

// Calcular totales y renderizar el carrito actualizado
function actualizarCarrito() {
    // Calcular cantidad total de productos
    const cantidadTotal = cart.reduce((acc, item) => acc + item.cantidad, 0);
    cartBadgeCount.textContent = cantidadTotal;
    
    // Renderizar ítems
    if (cart.length === 0) {
        listaCarritoItems.innerHTML = '<p class="empty-cart-message">Tu bolsa de compras está vacía.</p>';
        resumenValoresCarrito.style.display = 'none';
        return;
    }
    
    resumenValoresCarrito.style.display = 'block';
    listaCarritoItems.innerHTML = '';
    
    let subtotal = 0;
    
    cart.forEach(item => {
        const itemTotal = item.precio * item.cantidad;
        subtotal += itemTotal;
        
        const cartItemEl = document.createElement('div');
        cartItemEl.className = 'cart-item';
        cartItemEl.innerHTML = `
            <div class="cart-item-info">
                <div class="cart-item-name">${item.nombre}</div>
                <div class="cart-item-price">$${item.precio} c/u</div>
            </div>
            <div class="cart-item-controls">
                <button class="btn-qty btn-restar" data-id="${item.id}"><i class="fa-solid fa-minus"></i></button>
                <span class="cart-item-qty">${item.cantidad}</span>
                <button class="btn-qty btn-sumar" data-id="${item.id}"><i class="fa-solid fa-plus"></i></button>
            </div>
        `;
        
        // Listeners para cambiar cantidad
        cartItemEl.querySelector('.btn-restar').addEventListener('click', () => modificarCantidad(item.id, -1));
        cartItemEl.querySelector('.btn-sumar').addEventListener('click', () => modificarCantidad(item.id, 1));
        
        listaCarritoItems.appendChild(cartItemEl);
    });
    
    cartSubtotal.textContent = `$${subtotal}`;
    cartTotal.textContent = `$${subtotal}`;
}

// Control del Carrito Offcanvas
function abrirCarrito() {
    panelCarrito.classList.add('open');
    overlayCarrito.classList.add('open');
}

function cerrarCarrito() {
    panelCarrito.classList.remove('open');
    overlayCarrito.classList.remove('open');
}

// Control del Modal de Checkout
function abrirCheckout() {
    cerrarCarrito();
    modalCheckout.classList.add('open');
    document.getElementById('campo-nombre').focus();
}

function cerrarCheckout() {
    modalCheckout.classList.remove('open');
}

// Procesar el Pedido y Generar Enlace de WhatsApp
function procesarEnvioPedido(e) {
    e.preventDefault();
    
    const nombre = document.getElementById('campo-nombre').value.trim();
    const tipoEntrega = campoTipoEntrega.value;
    const direccion = campoDireccion.value.trim();
    const metodoPago = document.getElementById('campo-pago').value;
    const notas = document.getElementById('campo-notas').value.trim();
    
    // Validar carrito
    if (cart.length === 0) {
        mostrarToast('Tu carrito está vacío.');
        return;
    }
    
    // Formatear texto del pedido
    let mensaje = `☕ *¡Nuevo Pedido - Uno Catorce Café!* ☕\n`;
    mensaje += `---------------------------------\n`;
    mensaje += `👤 *Cliente:* ${nombre}\n`;
    mensaje += `🛵 *Tipo de Entrega:* ${tipoEntrega}\n`;
    
    if (tipoEntrega === 'Delivery') {
        mensaje += `📍 *Dirección:* ${direccion}\n`;
    }
    
    mensaje += `💳 *Método de Pago:* ${metodoPago}\n`;
    
    if (notas) {
        mensaje += `📝 *Aclaraciones:* _${notas}_\n`;
    }
    
    mensaje += `---------------------------------\n`;
    mensaje += `🛒 *Detalle del Pedido:*\n`;
    
    let total = 0;
    cart.forEach(item => {
        const itemTotal = item.precio * item.cantidad;
        total += itemTotal;
        mensaje += `• *${item.cantidad}x* ${item.nombre} (_$${item.precio}_) -> *$${itemTotal}*\n`;
    });
    
    mensaje += `---------------------------------\n`;
    mensaje += `💰 *TOTAL A PAGAR:* *$${total} UYU*\n`;
    
    // Codificar texto para URL
    const textoCodificado = encodeURIComponent(mensaje);
    
    // Número de teléfono de Uno Catorce (Simulado para pruebas, se puede reemplazar por el de producción)
    // El formato internacional es: código_país (598) + celular_sin_cero (94114114)
    const telefonoLocal = "59894114114"; // Celular simulado de la cafetería
    
    const urlWhatsApp = `https://wa.me/${telefonoLocal}?text=${textoCodificado}`;
    
    // Limpiar Carrito y Cerrar Modal antes de redirigir
    cart = [];
    actualizarCarrito();
    cerrarCheckout();
    
    // Redirigir a WhatsApp (abrirá en nueva pestaña o en app nativa)
    window.open(urlWhatsApp, '_blank');
    
    mostrarToast('¡Redirigiendo a WhatsApp para confirmar su pedido!');
}

// Mostrar Toast Informativo
function mostrarToast(mensaje) {
    toastMensaje.textContent = mensaje;
    toastNotificacion.classList.add('show');
    
    setTimeout(() => {
        toastNotificacion.classList.remove('show');
    }, 3500);
}
