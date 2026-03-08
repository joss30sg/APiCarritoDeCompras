import { Injectable } from '@angular/core';

/**
 * JWT Token Service - Gestor Centralizado de Tokens
 * 
 * Responsabilidad:
 * - Almacenar y recuperar tokens JWT de localStorage
 * - Decodificar payload del token para extraer información
 * - Validar expiración del token
 * - Manejar refresh tokens para renovación automática
 * 
 * Patrón: Singleton Pattern
 * 
 * localStorage Keys:
 * - jwt_token: Token principal para autenticación en cada petición
 * - refresh_token: Token para refrescar cuando el principal expira
 * 
 * Notas Importantes:
 * - Los tokens JWT están codificados en base64, NO están cifrados
 * - El payload es legible en el cliente (no guardes datos sensibles)
 * - El servidor verifica la firma del token
 * 
 * @example
 * // Guardar token después de login
 * jwtTokenService.setToken(authResponse.token);
 * 
 * // Verificar si existe token
 * if (jwtTokenService.hasToken()) {
 *   // Usuario autenticado
 * }
 * 
 * // Verificar expiración
 * if (jwtTokenService.isTokenExpired()) {
 *   // Token expirado, necesita refresh o nuevo login
 * }
 */
@Injectable({
  providedIn: 'root'
})
export class JwtTokenService {
  private readonly TOKEN_KEY = 'jwt_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly DEBUG = true; // Cambiar a false en producción para desactivar logs

  constructor() {
    if (this.DEBUG) {
      console.log('[JwtTokenService] Servicio inicializado');
    }
  }

  /**
   * Guardar token JWT en localStorage
   * 
   * Se llama después de login exitoso para almacenar el token
   * y emplearlo en futuras peticiones HTTP
   * 
   * @param token - Token JWT completo (formato: header.payload.signature)
   * 
   * @example
   * jwtTokenService.setToken('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...');
   */
  setToken(token: string): void {
    try {
      const decoded = this.decodeToken(token);
      if (this.DEBUG) {
        console.log('[JwtTokenService] Token guardado en localStorage', {
          usuario: decoded?.email || decoded?.sub || 'Desconocido',
          expiracion: decoded?.exp ? new Date(decoded.exp * 1000) : 'N/A',
          longitudToken: token.length,
          fechaGuardado: new Date()
        });
      }
      localStorage.setItem(this.TOKEN_KEY, token);
    } catch (error) {
      console.error('[JwtTokenService] Error guardando token', error);
    }
  }

  /**
   * Obtener token JWT de localStorage
   * 
   * @returns Token JWT completo o null si no existe
   * 
   * @example
   * const token = jwtTokenService.getToken();
   * if (token) {
   *   // Token disponible, puede usar para peticiones
   * } else {
   *   // No hay token, usuario no autenticado
   * }
   */
  getToken(): string | null {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (this.DEBUG && !token) {
      console.warn('[JwtTokenService] No se encontró token en localStorage');
    }
    return token;
  }

  /**
   * Guardar refresh token en localStorage
   * 
   * El refresh token se usa para obtener un nuevo token principal
   * cuando el anterior ha expirado, sin que el usuario deba login nuevamente
   * 
   * @param refreshToken - Token para refrescar
   */
  setRefreshToken(refreshToken: string): void {
    if (this.DEBUG) {
      console.log('[JwtTokenService] Refresh token guardado en localStorage');
    }
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  /**
   * Obtener refresh token de localStorage
   * 
   * @returns Refresh token o null si no existe
   */
  getRefreshToken(): string | null {
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
    if (this.DEBUG && !refreshToken) {
      console.warn('[JwtTokenService] No se encontró refresh token en localStorage');
    }
    return refreshToken;
  }

  /**
   * Remover ambos tokens de localStorage
   * 
   * Se llama cuando:
   * - Usuario realiza logout
   * - Token es inválido
   * - Interceptor detecta 401 (Unauthorized)
   * 
   * @example
   * jwtTokenService.removeToken(); // Limpia todos los tokens
   */
  removeToken(): void {
    if (this.DEBUG) {
      console.log('[JwtTokenService] Tokens eliminados de localStorage (logout)');
    }
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Verificar si existe un token en localStorage
   * 
   * Útil para determinar si el usuario está autenticado
   * 
   * @returns true si existe token, false si no
   * 
   * @example
   * if (jwtTokenService.hasToken()) {
   *   console.log('Usuario autenticado');
   * } else {
   *   // Redirigir a login
   * }
   */
  hasToken(): boolean {
    const exists = this.getToken() !== null;
    if (this.DEBUG) {
      console.log('[JwtTokenService] hasToken() →', exists);
    }
    return exists;
  }

  /**
   * Decodificar token JWT (extrae el payload)
   * 
   * Estructura JWT: header.payload.signature
   * Esta función extrae y decodifica el PAYLOAD
   * 
   * JWT está en base64 pero NO cifrado, es LEGIBLE en el cliente
   * Por eso NO GUARDES información sensible en el payload
   * 
   * Contenido típico del payload:
   * - sub: ID del usuario (Subject)
   * - email: Email del usuario
   * - name: Nombre del usuario
   * - iat: Issued At (timestamp de creación)
   * - exp: Expiration (timestamp de expiración)
   * - iss: Issuer (quién lo generó)
   * - aud: Audience (para quién está dirigido)
   * 
   * @param token - Token JWT completo
   * @returns Objeto con el payload del token o null si no es válido
   * 
   * @example
   * const payload = jwtTokenService.decodeToken(token);
   * if (payload) {
   *   console.log('Email usuario:', payload.email);
   *   console.log('Expira:', new Date(payload.exp * 1000));
   * } else {
   *   console.log('Token inválido');
   * }
   */
  decodeToken(token: string): any {
    try {
      // 1. Validar formato del token
      const parts = token.split('.');
      if (parts.length !== 3) {
        throw new Error('Token inválido: debe tener formato header.payload.signature');
      }

      // 2. Extraer la segunda parte (payload)
      const base64Url = parts[1];
      
      // 3. Convertir base64url a base64 estándar
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      
      // 4. Decodificar base64 a string
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      
      // 5. Parsear JSON
      const decoded = JSON.parse(jsonPayload);

      if (this.DEBUG) {
        console.log('[JwtTokenService] Token decodificado exitosamente', {
          usuario: decoded.email || decoded.sub || 'Desconocido',
          expiracion: decoded.exp ? new Date(decoded.exp * 1000) : 'Sin expiración',
          claims: Object.keys(decoded)
        });
      }

      return decoded;
    } catch (error) {
      console.error('[JwtTokenService] Error decodificando token', {
        error: (error as any).message,
        tokenLongitud: token.length
      });
      return null;
    }
  }

  /**
   * Verificar si el token está expirado
   * 
   * El token está expirado si: fecha actual >= fecha de expiración
   * 
   * El servidor verifica la expiración, pero el cliente también
   * puede verificarla para evitar peticiones innecesarias
   * 
   * @param token - Token JWT (opcional, usa el actual si no se proporciona)
   * @returns true si está expirado, false si todavía es válido
   * 
   * @example
   * if (jwtTokenService.isTokenExpired()) {
   *   console.log('Token expirado');
   *   // Intentar refrescar o redirigir a login
   * } else {
   *   console.log('Token válido');
   * }
   */
  isTokenExpired(token: string = this.getToken() || ''): boolean {
    if (!token) {
      if (this.DEBUG) {
        console.warn('[JwtTokenService] No hay token para verificar expiración');
      }
      return true;
    }

    try {
      const decoded = this.decodeToken(token);
      if (!decoded || !decoded.exp) {
        if (this.DEBUG) {
          console.warn('[JwtTokenService] Token no tiene propiedad exp (expiración)');
        }
        return true;
      }

      // exp está en segundos, convertir a milisegundos para comparar con Date.now()
      const expirationTime = decoded.exp * 1000;
      const isExpired = Date.now() >= expirationTime;

      if (this.DEBUG) {
        const remainingTime = expirationTime - Date.now();
        const remainingSeconds = Math.round(remainingTime / 1000);
        const remainingMinutes = Math.round(remainingSeconds / 60);
        const remainingHours = Math.round(remainingMinutes / 60);
        
        console.log('[JwtTokenService] Estado de expiración', {
          expirado: isExpired,
          fechaExpiracion: new Date(expirationTime),
          tiempoRestante: isExpired 
            ? 'Expirado'
            : remainingMinutes < 1 
              ? `${remainingSeconds} segundos`
              : remainingHours < 1
                ? `${remainingMinutes} minutos`
                : `${remainingHours} horas`
        });
      }

      return isExpired;
    } catch (error) {
      console.error('[JwtTokenService] Error verificando expiración', error);
      return true; // Si hay error, asumir que está expirado por seguridad
    }
  }

  /**
   * Obtener fecha de expiración del token
   * 
   * @param token - Token JWT (opcional, usa el actual si no se proporciona)
   * @returns Objeto Date con la fecha/hora de expiración, o null si no es válido
   * 
   * @example
   * const expirationDate = jwtTokenService.getTokenExpiration();
   * if (expirationDate) {
   *   console.log('Token expira el:', expirationDate.toLocaleString());
   * }
   */
  getTokenExpiration(token: string = this.getToken() || ''): Date | null {
    try {
      const decoded = this.decodeToken(token);
      if (!decoded || !decoded.exp) {
        if (this.DEBUG) {
          console.warn('[JwtTokenService] Token no tiene información de expiración');
        }
        return null;
      }
      
      const expirationDate = new Date(decoded.exp * 1000);

      if (this.DEBUG) {
        console.log('[JwtTokenService] Fecha de expiración:', expirationDate.toLocaleString());
      }

      return expirationDate;
    } catch (error) {
      console.error('[JwtTokenService] Error obteniendo fecha de expiración', error);
      return null;
    }
  }
}
// Token Service
