import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;  // allow for passing of data from parent (MemberDetailComponent to child (this))
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    //                    the + converts the nameid into a number data type
    const currentUserId = +this.authService.decodedToken.nameid;

    // the tap operator allows for an action on the returned collection
    // prior to the suscribe 
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
                    .pipe(
                      tap (messages => {
                        for (let i = 0; i < messages.length; i++) {
                          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
                            this.userService.markAsRead(currentUserId, messages[i].id);
                          }
                        }
                      })
                      )
                    .subscribe(messages => { this.messages = messages; },
                               error => { this.alertify.error(error); });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => { 
                                //debugger;
                                this.messages.unshift(message);
                                this.newMessage.content = ''; },
                    error => { this.alertify.error(error); });
  }
}
