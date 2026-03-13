import { Component, inject, OnInit, signal } from '@angular/core';
import { WeatherForecastService } from './Services/weather-forecast.service';
import { WeatherForecast } from './Models/WeatherForecast.interface';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly title = signal('ClientApp');
  readonly weatherForecastService = inject(WeatherForecastService);

  message = signal<string>('');
  isLoading = signal(false);
  weatherForecast = signal<WeatherForecast[] | null>(null);

  ngOnInit(): void {
    this.loadWeather();
  }

  loadWeather() {
    this.isLoading.set(true);
    this.message.set('');
    this.weatherForecastService.getGetWeatherForecast().subscribe({
      next: (data) => {
        this.weatherForecast.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.message.set(`Error: ${error.message}`);
        this.weatherForecast.set(null);
        this.isLoading.set(false);
      },
    });
  }
}
