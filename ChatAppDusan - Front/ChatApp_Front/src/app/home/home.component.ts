import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ChatService } from '../services/chat.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{

  userForm: FormGroup = new FormGroup({});
  submitted = false;
  apiErrorMessages: string[] = [];
  openChat = false;


  constructor(private formBuilder: FormBuilder, private chatService: ChatService) { }
  
  ngOnInit(): void {
    this.initalizeForm();
  }

  initalizeForm() {
    this.userForm = this.formBuilder.group({
      name: ["", [Validators.required, Validators.minLength(5), Validators.maxLength(20)]]
    })
  }

  submitForm() {
    this.submitted = true;
    this.apiErrorMessages = [];

    if (this.userForm.valid) {
      this.chatService.registerUser(this.userForm.value).subscribe({
        next: () => {
          this.chatService.myName = this.userForm.get('name')?.value;
          this.openChat = true;
          this.userForm.reset();
          this.submitted = false;
        },
        error: error => {
          if (typeof (error.error) !== 'object') {
            this.apiErrorMessages.push(error.error);
          }
        }
      })
    }
  }

  closeChat() {
    this.openChat = false;
  }
}
