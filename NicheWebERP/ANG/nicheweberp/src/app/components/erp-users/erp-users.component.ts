import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ErpUserService } from '../../services/erp-user.service';
import {
  CreateErpUserRequest,
  ErpRole,
  ErpUser,
  UpdateErpUserRequest
} from '../../models/erp-user.model';

type FormMode = 'create' | 'edit';

interface UserFormModel {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  roleId: number | null;
  isActive: boolean;
}

@Component({
  selector: 'app-erp-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './erp-users.component.html',
  styleUrls: ['./erp-users.component.css', '../../shared/list-page.css']
})
export class ErpUsersComponent implements OnInit {
  users: ErpUser[] = [];
  roles: ErpRole[] = [];
  loading = true;
  error = false;

  showForm = false;
  formMode: FormMode = 'create';
  editingUserId: number | null = null;
  form: UserFormModel = this.emptyForm();
  saving = false;
  formError = '';

  resetPasswordUserId: number | null = null;
  resetPasswordValue = '';
  resetting = false;
  resetError = '';

  constructor(private erpUserService: ErpUserService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    forkJoin({
      users: this.erpUserService.getAllUsers(),
      roles: this.erpUserService.getAllRoles()
    }).subscribe({
      next: ({ users, roles }) => {
        this.users = users;
        this.roles = roles;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  roleName(roleId: number): string {
    return this.roles.find((r) => r.id === roleId)?.name || '—';
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingUserId = null;
    this.form = this.emptyForm();
    this.formError = '';
    this.showForm = true;
  }

  openEditForm(user: ErpUser): void {
    this.formMode = 'edit';
    this.editingUserId = user.id;
    this.form = {
      firstName: user.firstName,
      lastName: user.lastName,
      username: user.username,
      email: user.email,
      password: '',
      roleId: user.roleId,
      isActive: user.isActive
    };
    this.formError = '';
    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
  }

  submitForm(): void {
    if (!this.form.firstName || !this.form.lastName || !this.form.email || !this.form.roleId) {
      this.formError = 'First name, last name, email, and role are required.';
      return;
    }

    this.saving = true;
    this.formError = '';

    if (this.formMode === 'create') {
      if (!this.form.username || !this.form.password) {
        this.formError = 'Username and password are required.';
        this.saving = false;
        return;
      }
      const request: CreateErpUserRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        username: this.form.username,
        email: this.form.email,
        password: this.form.password,
        roleId: this.form.roleId,
        locationId: null
      };
      this.erpUserService.createUser(request).subscribe({
        next: () => {
          this.saving = false;
          this.showForm = false;
          this.fetch();
        },
        error: (err) => this.handleFormError(err)
      });
    } else if (this.editingUserId !== null) {
      const request: UpdateErpUserRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        email: this.form.email,
        roleId: this.form.roleId,
        locationId: null,
        isActive: this.form.isActive
      };
      this.erpUserService.updateUser(this.editingUserId, request).subscribe({
        next: () => {
          this.saving = false;
          this.showForm = false;
          this.fetch();
        },
        error: (err) => this.handleFormError(err)
      });
    }
  }

  private handleFormError(err: { error?: { message?: string } }): void {
    this.saving = false;
    this.formError = err?.error?.message || 'Something went wrong. Please try again.';
  }

  openResetPassword(user: ErpUser): void {
    this.resetPasswordUserId = user.id;
    this.resetPasswordValue = '';
    this.resetError = '';
  }

  cancelResetPassword(): void {
    this.resetPasswordUserId = null;
  }

  submitResetPassword(): void {
    if (this.resetPasswordUserId === null) {
      return;
    }
    if (!this.resetPasswordValue || this.resetPasswordValue.length < 8) {
      this.resetError = 'New password must be at least 8 characters.';
      return;
    }

    this.resetting = true;
    this.resetError = '';
    this.erpUserService.resetPassword(this.resetPasswordUserId, this.resetPasswordValue).subscribe({
      next: () => {
        this.resetting = false;
        this.resetPasswordUserId = null;
      },
      error: () => {
        this.resetting = false;
        this.resetError = 'Could not reset the password. Please try again.';
      }
    });
  }

  private emptyForm(): UserFormModel {
    return {
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      password: '',
      roleId: this.roles[0]?.id ?? null,
      isActive: true
    };
  }
}
