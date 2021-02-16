import { Component } from '@angular/core';
import { NotificationService } from 'src/services/notification.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private response;
  title = 'PushNotificationApp';
  constructor(
    private _notification: NotificationService) { 
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
      this.checkResponses();
      
    });

  }
  checkResponses() {
    this._notification.checkResponse().subscribe(data => {
      this.response=data;
      alert(this.response.messages);
    });

  }

}
