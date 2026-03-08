/**
 * API Routes Constants
 * Constantes de rutas de la API
 */
export const API_ROUTES = {
  AUTH: {
    LOGIN: 'auth/login',
    REGISTER: 'auth/register',
    LOGOUT: 'auth/logout',
    REFRESH: 'auth/refresh'
  },
  PRODUCTS: {
    GET_ALL: 'products',
    GET_BY_ID: 'products/:id',
    SEARCH: 'products/search',
    AVAILABLE: 'products/available'
  },
  SHOPPING_CART: {
    GET: 'shopping-carts/user/:userId',
    ADD_ITEM: 'shopping-carts/:id/items',
    REMOVE_ITEM: 'shopping-carts/:id/items/:itemId',
    UPDATE_QUANTITY: 'shopping-carts/:id/items/:itemId',
    CLEAR: 'shopping-carts/:id/clear'
  },
  USERS: {
    GET_ALL: 'users',
    GET_BY_ID: 'users/:id',
    GET_BY_EMAIL: 'users/email/:email'
  }
};
