import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { User } from '../models/user';
import { HttpClient } from '@angular/common/http';
import { HubConnection } from '@microsoft/signalr';
import { HubConnectionBuilder } from '@microsoft/signalr/dist/esm/HubConnectionBuilder';
import { Message } from '../models/message';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrivateChatComponent } from '../private-chat/private-chat.component';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  myName: string = '';
  private chatConnection?: HubConnection;
  onlineUsers: string[] = [];
  messages: Message[] = [];
  privateMessages: Message[] = [];
  privateMessageInitiated = false;

  constructor(private httpClient: HttpClient, private modalService: NgbModal) { }

  registerUser(user: User) {
    return this.httpClient.post(`${environment.apiUrl}api/chat/register-user`, user, {responseType: 'text'});
  }

  createChatConnection() {
    this.chatConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}hubs/chat`).withAutomaticReconnect().build();
    
    this.chatConnection.start().catch(error => {
      console.log(error);
    });

    this.chatConnection.on("KorisnikPovezan", () => {
      this.addUserConnectionId();
    });

    this.chatConnection.on("OnlineUsers", (onlineUsers) => {
      this.onlineUsers = [...onlineUsers];
    });

    this.chatConnection.on('NovaPoruka', (newMessage: Message) => {
      this.messages = [...this.messages, newMessage];
    });

    this.chatConnection.on('OpenPrivateChat', (newMessage: Message) => {
      this.privateMessages = [...this.privateMessages, newMessage];
      this.privateMessageInitiated = true;
      const modalRef = this.modalService.open(PrivateChatComponent);
      modalRef.componentInstance.toUser = newMessage.from;
    });

    this.chatConnection.on('NewPrivateMessage', (newMessage: Message) => {
      this.privateMessages = [...this.privateMessages, newMessage];
    });

    this.chatConnection.on('ClosePrivateChat', () => {
      this.privateMessageInitiated = false;
      this.privateMessages = [];
      this.modalService.dismissAll();
    });
  }

  stopChatConnection() {
    this.chatConnection?.stop().catch(error => console.log(error));
  }

  async addUserConnectionId() {
    return this.chatConnection?.invoke('AddUserConnectionId', this.myName)
      .catch(error => console.log(error));
  }

  async sendMessage(content: string) {
    const message: Message = {
      from: this.myName,
      content
    };

    return this.chatConnection?.invoke('ReceiveMessage', message)
      .catch(error => console.log(error));
  }

  async sendPrivateMessage(to: string, content: string) {
    const message: Message = {
      from: this.myName,
      to,
      content
    };

    if (!this.privateMessageInitiated) {
      this.privateMessageInitiated = true;
      return this.chatConnection?.invoke('CreatePrivateChat', message).then(() => {
        this.privateMessages = [...this.privateMessages, message]
      })
        .catch(error => console.log(error));
    } else {
      return this.chatConnection?.invoke('ReceivePrivateMessage', message)
        .catch(error => console.log(error));
    }
  }

  async closePrivateChatMessage(otherUser: string) {
    return this.chatConnection?.invoke('RemovePrivateChat', this.myName, otherUser)
      .catch(error => console.log(error));
  }
}
