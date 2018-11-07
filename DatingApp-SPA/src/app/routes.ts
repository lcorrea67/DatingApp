import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent},
    {
        path: '',  // keep empty so the routes below will match, we could put in a value then route would be localhost :
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver} },  // , canActivate: [AuthGuard]
            // tslint:disable-next-line:max-line-length
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver}},  // , canActivate: [AuthGuard]
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
     },
    { path: '**', redirectTo: '', pathMatch: 'full' },
]