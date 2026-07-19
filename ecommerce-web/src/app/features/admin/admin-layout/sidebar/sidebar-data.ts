import { NavItem } from './nav-item';

export const adminNavItems: NavItem[] = [
  { navCap: 'Home' },
  {
    displayName: 'Dashboard',
    iconName: 'layout-grid-add',
    route: '/admin/dashboard'
  },
  { navCap: 'Catalog' },
  {
    displayName: 'Products',
    iconName: 'box',
    route: '/admin/products'
  },
  {
    displayName: 'Categories',
    iconName: 'list-details',
    route: '/admin/categories'
  },
  {
    displayName: 'Gift rules',
    iconName: 'gift',
    route: '/admin/gift-rules'
  },
  { navCap: 'Sales' },
  {
    displayName: 'Orders',
    iconName: 'shopping-cart',
    route: '/admin/orders'
  },
  {
    displayName: 'Customers',
    iconName: 'users',
    route: '/admin/customers'
  },
  { navCap: 'System' },
  {
    displayName: 'Audit log',
    iconName: 'file-text',
    route: '/admin/audit-logs'
  }
];
