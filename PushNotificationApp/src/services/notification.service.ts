import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from 'src/app/Model/User';
@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private http: HttpClient) { }

  private baseUrl= 'http://localhost:63300/';
  public loginsuccess(username, password): Observable<User> {
    return this.http.post<any>((this.baseUrl + 'api/notification/login'), { username, password })
  }

  public checkResponse(): Observable<User> {
    return this.http.get<any>(('http://localhost:63200/api/notification/consume'))
  }


}
