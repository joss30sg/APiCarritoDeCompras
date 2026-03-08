// === Swagger UI JavaScript Personalización ===

document.addEventListener('DOMContentLoaded', function() {
  // Personalizar el título de la página
  document.title = 'Shopping Cart API - Documentación Profesional';
  
  // Agregar información visual personalizada
  const infoSection = document.querySelector('.info');
  if (infoSection) {
    // Mejorar la presentación del título
    const titleElement = infoSection.querySelector('.title');
    if (titleElement) {
      titleElement.style.color = '#2c3e50';
      titleElement.style.borderBottom = '3px solid #0066cc';
      titleElement.style.paddingBottom = '10px';
    }
  }
  
  // Agregar atajos de teclado útiles
  document.addEventListener('keydown', function(e) {
    // Ctrl+/ para abrir la búsqueda
    if (e.ctrlKey && e.key === '/') {
      e.preventDefault();
      const searchInput = document.querySelector('.topbar-search input');
      if (searchInput) {
        searchInput.focus();
      }
    }
  });
  
  // Mejorar la visualización de códigos de error
  const statusCodes = {
    '200': { color: '#28a745', label: 'Éxito - Datos retornados' },
    '201': { color: '#49cc90', label: 'Creado - Recurso creado exitosamente' },
    '204': { color: '#61affe', label: 'Sin contenido - Operación exitosa' },
    '400': { color: '#fca130', label: 'Error de validación' },
    '401': { color: '#f93e3e', label: 'No autenticado - Token requerido' },
    '403': { color: '#e91e63', label: 'Prohibido - Acceso denegado' },
    '500': { color: '#f93e3e', label: 'Error del servidor' }
  };
  
  // Aplicar colores a los códigos de respuesta
  document.querySelectorAll('.response-col_status').forEach(function(element) {
    const code = element.textContent.trim().match(/\d{3}/);
    if (code && statusCodes[code[0]]) {
      element.style.color = statusCodes[code[0]].color;
      element.style.fontWeight = 'bold';
    }
  });
  
  // Función para copiar URL al portapapeles
  function setupClipboardCopy() {
    const operationElements = document.querySelectorAll('.opblock-summary-path');
    operationElements.forEach(function(element) {
      element.style.cursor = 'copy';
      element.title = 'Clic para copiar la ruta';
      element.addEventListener('click', function(e) {
        if (e.target === element) {
          const path = element.textContent.trim();
          navigator.clipboard.writeText(path);
          showNotification('Ruta copiada al portapapeles: ' + path);
        }
      });
    });
  }
  
  // Mostrar notificación temporal
  function showNotification(message) {
    const notification = document.createElement('div');
    notification.textContent = message;
    notification.style.cssText = `
      position: fixed;
      top: 80px;
      right: 20px;
      background: #28a745;
      color: white;
      padding: 12px 20px;
      border-radius: 4px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.2);
      z-index: 10000;
      animation: slideIn 0.3s ease;
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(function() {
      notification.style.animation = 'slideOut 0.3s ease';
      setTimeout(function() {
        notification.remove();
      }, 300);
    }, 3000);
  }
  
  // Agregar estilos de animación
  const style = document.createElement('style');
  style.textContent = `
    @keyframes slideIn {
      from {
        transform: translateX(400px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }
    
    @keyframes slideOut {
      from {
        transform: translateX(0);
        opacity: 1;
      }
      to {
        transform: translateX(400px);
        opacity: 0;
      }
    }
    
    .opblock-summary-path {
      user-select: all;
    }
    
    .copy-button {
      cursor: pointer;
      opacity: 0.6;
      transition: opacity 0.3s;
    }
    
    .copy-button:hover {
      opacity: 1;
    }
  `;
  document.head.appendChild(style);
  
  // Configurar eventos de clic en los operaciones
  const observer = new MutationObserver(function(mutations) {
    setupClipboardCopy();
  });
  
  observer.observe(document.body, {
    childList: true,
    subtree: true,
    attributes: false,
    characterData: false
  });
  
  // Llamada inicial
  setupClipboardCopy();
  
  // Mejorar la visualización de esquemas
  addSchemaIcons();
  
  // Agregar información de versión de API
  addVersionInfo();
});

// Agregar iconos a los esquemas
function addSchemaIcons() {
  const schemaLabels = document.querySelectorAll('.model-title');
  schemaLabels.forEach(function(label) {
    if (!label.querySelector('.schema-icon')) {
      const icon = document.createElement('span');
      icon.className = 'schema-icon';
      icon.textContent = '📋 ';
      icon.style.marginRight = '8px';
      label.insertBefore(icon, label.firstChild);
    }
  });
}

// Agregar información de versión
function addVersionInfo() {
  const topbar = document.querySelector('.topbar-wrapper');
  if (topbar && !topbar.querySelector('.api-version')) {
    const versionDiv = document.createElement('div');
    versionDiv.className = 'api-version';
    versionDiv.innerHTML = `
      <small style="color: #999; margin-left: 10px; font-size: 12px;">
        Versión: v1.0.0 | Estado: Production | Última actualización: marzo 2026
      </small>
    `;
    topbar.appendChild(versionDiv);
  }
}

// Función para expandir/contraer todos los endpoints
window.toggleAllEndpoints = function(expand) {
  const opblocks = document.querySelectorAll('.opblock');
  opblocks.forEach(function(block) {
    const isOpen = block.classList.contains('open');
    if ((expand && !isOpen) || (!expand && isOpen)) {
      block.querySelector('.opblock-summary').click();
    }
  });
};

// Agregar botones de utilidad en la interfaz
document.addEventListener('DOMContentLoaded', function() {
  setTimeout(function() {
    const topbar = document.querySelector('.topbar-right');
    if (topbar) {
      const utilsDiv = document.createElement('div');
      utilsDiv.className = 'swagger-utils';
      utilsDiv.innerHTML = `
        <button onclick="window.toggleAllEndpoints(true)" style="
          margin-right: 8px;
          padding: 6px 12px;
          background: #0066cc;
          color: white;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-size: 12px;
        " title="Expandir todos los endpoints">
          📂 Expandir Todo
        </button>
        <button onclick="window.toggleAllEndpoints(false)" style="
          padding: 6px 12px;
          background: #666;
          color: white;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-size: 12px;
        " title="Contraer todos los endpoints">
          📁 Contraer Todo
        </button>
      `;
      topbar.insertBefore(utilsDiv, topbar.firstChild);
    }
  }, 1000);
});

console.log('✅ Swagger UI personalización cargada correctamente');
