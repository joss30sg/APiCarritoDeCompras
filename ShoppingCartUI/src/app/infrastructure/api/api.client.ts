import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

/**
 * API Client - Fachada HTTP Centralizada
 * 
 * Responsabilidad:
 * - Centralizar todas las peticiones HTTP a la API
 * - Manejar la construcción de URLs
 * - Proporcionar métodos reutilizables (GET, POST, PUT, PATCH, DELETE)
 * 
 * Patrón: Facade Pattern + Service Locator
 * 
 * Uso:
 * ```typescript
 * constructor(private apiClient: ApiClient) {}
 * 
 * // GET
 * const users = await this.apiClient.get<User[]>('users');
 * 
 * // POST
 * const newUser = await this.apiClient.post<User>('users', userData);
 * 
 * // PUT
 * const updated = await this.apiClient.put<User>('users/1', userData);
 * 
 * // DELETE
 * await this.apiClient.delete('users/1');
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class ApiClient {
  private apiUrl = environment.apiUrl;
  private readonly DEBUG = !environment.production; // Habilitar logs en desarrollo

  constructor(private http: HttpClient) {
    this.log('ApiClient inicializado', { apiUrl: this.apiUrl });
  }

  /**
   * GET - Obtener datos
   * 
   * @param endpoint - Ruta del endpoint (ej: 'users', 'products/1')
   * @returns Promise con los datos del tipo T
   * 
   * @example
   * const user = await this.apiClient.get<User>('users/1');
   */
  get<T>(endpoint: string): Promise<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    this.log('GET', { endpoint, url });

    return this.http.get<T>(url)
      .toPromise()
      .then(data => {
        this.log('GET exitoso', { endpoint, data });
        return data as T;
      })
      .catch(error => {
        this.error('GET fallido', { endpoint, error });
        throw error;
      });
  }

  /**
   * POST - Crear nuevo recurso
   * 
   * @param endpoint - Ruta del endpoint (ej: 'users')
   * @param body - Datos a enviar
   * @returns Promise con el recurso creado
   * 
   * @example
   * const newUser = await this.apiClient.post<User>('users', { name: 'Juan' });
   */
  post<T>(endpoint: string, body: any): Promise<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    this.log('POST', { endpoint, url, body });

    return this.http.post<T>(url, body)
      .toPromise()
      .then(data => {
        this.log('POST exitoso', { endpoint, data });
        return data as T;
      })
      .catch(error => {
        this.error('POST fallido', { endpoint, error });
        throw error;
      });
  }

  /**
   * PUT - Actualizar recurso completo
   * 
   * @param endpoint - Ruta del endpoint (ej: 'users/1')
   * @param body - Datos actualizados
   * @returns Promise con el recurso actualizado
   * 
   * @example
   * const updated = await this.apiClient.put<User>('users/1', { name: 'Carlos' });
   */
  put<T>(endpoint: string, body: any): Promise<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    this.log('PUT', { endpoint, url, body });

    return this.http.put<T>(url, body)
      .toPromise()
      .then(data => {
        this.log('PUT exitoso', { endpoint, data });
        return data as T;
      })
      .catch(error => {
        this.error('PUT fallido', { endpoint, error });
        throw error;
      });
  }

  /**
   * PATCH - Actualizar parcialmente un recurso
   * 
   * @param endpoint - Ruta del endpoint (ej: 'users/1')
   * @param body - Datos parciales a actualizar
   * @returns Promise con el recurso actualizado
   * 
   * @example
   * const updated = await this.apiClient.patch<User>('users/1', { email: 'nuevo@email.com' });
   */
  patch<T>(endpoint: string, body: any): Promise<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    this.log('PATCH', { endpoint, url, body });

    return this.http.patch<T>(url, body)
      .toPromise()
      .then(data => {
        this.log('PATCH exitoso', { endpoint, data });
        return data as T;
      })
      .catch(error => {
        this.error('PATCH fallido', { endpoint, error });
        throw error;
      });
  }

  /**
   * DELETE - Eliminar recurso
   * 
   * @param endpoint - Ruta del endpoint (ej: 'users/1')
   * @returns Promise void
   * 
   * @example
   * await this.apiClient.delete('users/1');
   */
  delete<T>(endpoint: string): Promise<T> {
    const url = `${this.apiUrl}/${endpoint}`;
    this.log('DELETE', { endpoint, url });

    return this.http.delete<T>(url)
      .toPromise()
      .then(data => {
        this.log('DELETE exitoso', { endpoint });
        return data as T;
      })
      .catch(error => {
        this.error('DELETE fallido', { endpoint, error });
        throw error;
      });
  }

  /**
   * Log helper para debugging
   * Solo se ejecuta si DEBUG está habilitado
   */
  private log(message: string, context?: any): void {
    if (this.DEBUG) {
      console.log(`[ApiClient] ${message}`, context);
    }
  }

  /**
   * Error logger para debugging
   */
  private error(message: string, context?: any): void {
    console.error(`[ApiClient] ${message}`, context);
  }
}
