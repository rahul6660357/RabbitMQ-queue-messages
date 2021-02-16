import { Component } from '@angular/core';
import { NotificationService } from 'src/services/notification.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'PushNotificationApp';
  constructor(
    private _notification: NotificationService) { 
  }
 private user:any;
 username;
 password;
  login() {
    this._notification.loginsuccess(this.username, this.password).subscribe(data => {
      this.user=data;
      alert(this.user.messages);
    });

  }
}
