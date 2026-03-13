import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { retry, timer } from 'rxjs';

export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  // Solo reintentar GET
  if (req.method !== 'GET') {
    return next(req);
  }

  //retry({count: 2, delay: this.shouldRetry})
  // count: es la cantidad de reintentos
  // delay: es el tiempo entre reintentos
  return next(req).pipe(
    retry({
      count: 2,
      delay: shouldRetry,
    }),
  );
};

function shouldRetry(error: HttpErrorResponse, retryCount: number) {
  // Error de red (Cloudflare / túnel / conexión)
  if (error.status === 0) {
    const delayTime = retryCount * 1500; // 1.5s, 3s, 4.5s
    console.log(`Network error. Reintentando en ${delayTime}ms...`);
    return timer(delayTime);
  }

  // Error del servidor
  if (error.status >= 500) {
    const delayTime = 2000; // fijo y más corto
    console.log('Error 5xx. Reintentando...');
    return timer(delayTime);
  }

  //de lo contrario devolvemos el error
  throw error;
}
