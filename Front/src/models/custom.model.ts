export class UserInfoModel {
    firstName = '';
    lastName = '';
    email = '';
    phoneNumber = '';
    role = '';
    onLeave = null;
    leaveReason  = '';
  }
  export class StatusModel {
    id?: string | undefined = '1';
    name?: string | undefined = 'In Progress';
    colour?: string | undefined;
    bgcolour?: string | undefined;
}
