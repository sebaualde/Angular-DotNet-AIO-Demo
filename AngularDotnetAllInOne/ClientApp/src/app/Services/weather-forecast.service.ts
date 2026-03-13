import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { WeatherForecast } from '../Models/WeatherForecast.interface';

@Injectable({
  providedIn: 'root',
})
export class WeatherForecastService {
  private readonly apiURL = environment.apiUrl + 'WeatherForecast';

  private httpClient = inject(HttpClient);

  public getGetWeatherForecast() {
    return this.httpClient.get<WeatherForecast[]>(this.apiURL);
  }
}
