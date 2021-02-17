import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr"; 
import { User } from 'src/app/Model/User';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  constructor() { }
  public data:User;
  public response;
  private hubConnection: signalR.HubConnection
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('http://localhost:63200/consume')
                            .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }
  public addLoginListener = () => {
    this.hubConnection.on('login', (data) => {
      console.log("Login queue processed");
      this.response=data;
        console.log(this.response);
        alert(this.response.messages);
    });
  }
}
