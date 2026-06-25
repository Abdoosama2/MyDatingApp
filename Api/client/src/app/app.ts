import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { inject } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
  

export class App implements OnInit {
 
  private http = inject(HttpClient)
  protected readonly title = 'dating app';
  protected users: any;
  ngOnInit(): void {

    this.http.get(`https://localhost:7114/api/user`).subscribe({
      next: ((res) => {
        console.log(res);
        this.users = res;
      }),
      error: ((error) => {
        console.log(error);
      }),
      complete: (() => {
        console.log("api request completed");
      })
    })
  }
}
