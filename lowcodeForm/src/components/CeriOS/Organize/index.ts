import { withInstall } from '@/utils';
import OrganizeSelect from './src/OrganizeSelect.vue';
import OrganizeSelectAsync from './src/OrganizeSelectAsync.vue';
import DepSelect from './src/DepSelect.vue';
import DepSelectAsync from './src/DepSelectAsync.vue';
import PosSelect from './src/PosSelect.vue';
import GroupSelect from './src/GroupSelect.vue';
import RoleSelect from './src/RoleSelect.vue';
import UserSelect from './src/UserSelect.vue';
import UsersSelect from './src/UsersSelect.vue';

const isAsync = true;

export const CeriOrganizeSelect = withInstall(isAsync ? OrganizeSelectAsync : OrganizeSelect);
export const CeriDepSelect = withInstall(isAsync ? DepSelectAsync : DepSelect);
export const CeriPosSelect = withInstall(PosSelect);
export const CeriGroupSelect = withInstall(GroupSelect);
export const CeriRoleSelect = withInstall(RoleSelect);
export const CeriUserSelect = withInstall(UserSelect);
export const CeriUsersSelect = withInstall(UsersSelect);
