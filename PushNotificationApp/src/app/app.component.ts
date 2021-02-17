import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { NotificationService } from 'src/services/notification.service';
import { SignalRService } from 'src/services/signal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private response;
  title = 'PushNotificationApp';
  constructor(
    private _notification: NotificationService,
    public signalRService: SignalRService,
    private http: HttpClient) { 
  }
  ngOnInit(){
   
  }
 private user:any;
 username;
 password;
  login() {
    this._notification.loginsuccess(this.username, this.password).subscribe(data => {
      this.user=data;
      console.log(this.user);
      alert(this.user.messages);
       this.signalRService.startConnection();
      this.signalRService.addLoginListener();   
      this.startHttpRequest();
      
    });

  }

  private startHttpRequest = () => {
    this.http.get('http://localhost:63200/api/notification/consume')
      .subscribe(data => {
        console.log(data);
      })
  }

}
