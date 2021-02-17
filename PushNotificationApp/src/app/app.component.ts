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
    this.signalRService.startConnection();
    //this.signalRService.addTransferChartDataListener();   
    this.startHttpRequest();
  }
 private user:any;
 username;
 password;
  login() {
    this._notification.loginsuccess(this.username, this.password).subscribe(data => {
      this.user=data;
      console.log(this.user);
      alert(this.user.messages);
     // this.checkResponses();
      
    });

  }
  // checkResponses() {
  //   this._notification.checkResponse().subscribe(data => {
  //     this.response=data;
  //     alert(this.response.messages);
  //   });

  //}
  private startHttpRequest = () => {
    this.http.get('http://localhost:63200/api/notification/consume')
      .subscribe(res => {
        console.log(res);
      })
  }

}
