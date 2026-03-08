import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { JwtTokenService } from '../../infrastructure/interceptors/jwt-token.service';
import { LoginRequest, RegisterRequest } from '../../shared/models/auth.model';

/**
 * Pruebas Unitarias para AuthService
 * 
 * Cubre:
 * - Calls HTTP a login y register
 * - Obtención y manejo de tokens
 * - Validación de autenticación
 * - Logout
 * 
 * Patrón: AAA (Arrange, Act, Assert)
 * Tools: HttpClientTestingModule para mocklist HTTP calls
 */
describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let jwtTokenService: JwtTokenService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService, JwtTokenService]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    jwtTokenService = TestBed.inject(JwtTokenService);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  describe('login', () => {
    it('should make HTTP POST request to login endpoint', () => {
      // Arrange
      const credentials: LoginRequest = {
        username: 'testuser',
        password: 'TestPassword123!'
      };

      const mockResponse = {
        data: {
          token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlciIsImV4cCI6OTk5OTk5OTk5OX0.signature',
          refreshToken: 'refresh-token-123',
          user: { id: '1', username: 'testuser' }
        }
      };

      // Act
      service.login(credentials).subscribe(result => {
        // Assert
        expect(result).toEqual(mockResponse);
      });

      // HTTP call efectivo
      const req = httpMock.expectOne('http://localhost:5276/api/v1/auth/login');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(credentials);
      req.flush(mockResponse);
    });

    it('should handle login errors', () => {
      // Arrange
      const credentials: LoginRequest = {
        username: 'testuser',
        password: 'WrongPassword123!'
      };

      // Act & Assert
      service.login(credentials).subscribe(
        () => fail('should have failed'),
        (error) => {
          expect(error.status).toBe(401);
        }
      );

      const req = httpMock.expectOne('http://localhost:5276/api/v1/auth/login');
      req.flush({ message: 'Invalid credentials' }, { status: 401, statusText: 'Unauthorized' });
    });
  });

  describe('register', () => {
    it('should make HTTP POST request to register endpoint', () => {
      // Arrange
      const newUser: RegisterRequest = {
        username: 'newuser',
        password: 'NewPassword123!'
      };

      const mockResponse = {
        data: {
          token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJuZXd1c2VyIiwic3ViIjoiMiJ9.signature',
          refreshToken: 'refresh-token-456',
          user: { id: '2', username: 'newuser' }
        }
      };

      // Act
      service.register(newUser).subscribe(result => {
        // Assert
        expect(result).toEqual(mockResponse);
      });

      // HTTP call efectivo
      const req = httpMock.expectOne('http://localhost:5276/api/v1/auth/register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newUser);
      req.flush(mockResponse);
    });

    it('should handle register errors', () => {
      // Arrange
      const newUser: RegisterRequest = {
        username: 'existinguser',
        password: 'Password123!'
      };

      // Act & Assert
      service.register(newUser).subscribe(
        () => fail('should have failed'),
        (error) => {
          expect(error.status).toBe(400);
        }
      );

      const req = httpMock.expectOne('http://localhost:5276/api/v1/auth/register');
      req.flush({ message: 'Username already exists' }, { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('Token Management', () => {
    it('should get token from localStorage', () => {
      // Arrange
      const testToken = 'test-token-123';
      localStorage.setItem('jwt_token', testToken);

      // Act
      const result = service.getToken();

      // Assert
      expect(result).toBe(testToken);
    });

    it('should return null when no token in localStorage', () => {
      // Act
      const result = service.getToken();

      // Assert
      expect(result).toBeNull();
    });

    it('should get refresh token from localStorage', () => {
      // Arrange
      const testRefreshToken = 'refresh-token-456';
      localStorage.setItem('refresh_token', testRefreshToken);

      // Act
      const result = service.getRefreshToken();

      // Assert
      expect(result).toBe(testRefreshToken);
    });
  });

  describe('isAuthenticated', () => {
    it('should return true if valid and non-expired token exists', () => {
      // Arrange
      // Create a non-expired JWT (exp in future)
      const futureExpiry = Math.floor(Date.now() / 1000) + 3600; // 1 hour from now
      const validToken = `header.${btoa(JSON.stringify({ exp: futureExpiry }))}.signature`;
      
      spyOn(jwtTokenService, 'getToken').and.returnValue(validToken);
      spyOn(jwtTokenService, 'isTokenExpired').and.returnValue(false);

      // Act
      const result = service.isAuthenticated();

      // Assert
      expect(result).toBe(true);
    });

    it('should return false if no token exists', () => {
      // Arrange
      spyOn(jwtTokenService, 'getToken').and.returnValue(null);

      // Act
      const result = service.isAuthenticated();

      // Assert
      expect(result).toBe(false);
    });

    it('should return false if token is expired', () => {
      // Arrange
      spyOn(jwtTokenService, 'getToken').and.returnValue('expired-token');
      spyOn(jwtTokenService, 'isTokenExpired').and.returnValue(true);

      // Act
      const result = service.isAuthenticated();

      // Assert
      expect(result).toBe(false);
    });
  });

  describe('isTokenExpired', () => {
    it('should return true if token is expired', () => {
      // Arrange
      spyOn(jwtTokenService, 'isTokenExpired').and.returnValue(true);

      // Act
      const result = service.isTokenExpired('expired-token');

      // Assert
      expect(result).toBe(true);
    });

    it('should return false if token is valid', () => {
      // Arrange
      spyOn(jwtTokenService, 'isTokenExpired').and.returnValue(false);

      // Act
      const result = service.isTokenExpired('valid-token');

      // Assert
      expect(result).toBe(false);
    });
  });

  describe('getTokenExpiration', () => {
    it('should return expiration date if token has exp claim', () => {
      // Arrange
      const futureDate = new Date(Date.now() + 3600000);
      spyOn(jwtTokenService, 'getTokenExpiration').and.returnValue(futureDate);

      // Act
      const result = service.getTokenExpiration();

      // Assert
      expect(result).toEqual(futureDate);
    });

    it('should return null if token is invalid', () => {
      // Arrange
      spyOn(jwtTokenService, 'getTokenExpiration').and.returnValue(null);

      // Act
      const result = service.getTokenExpiration();

      // Assert
      expect(result).toBeNull();
    });
  });

  describe('logout', () => {
    it('should remove tokens from localStorage', () => {
      // Arrange
      localStorage.setItem('jwt_token', 'test-token');
      localStorage.setItem('refresh_token', 'test-refresh-token');
      spyOn(jwtTokenService, 'removeToken');

      // Act
      service.logout();

      // Assert
      expect(jwtTokenService.removeToken).toHaveBeenCalled();
    });
  });
});
