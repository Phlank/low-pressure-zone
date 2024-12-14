import { ref, type Ref } from 'vue'

export interface NavigationItem {
  isActive: boolean
  title: NavigationTarget
  path: string
}

export enum NavigationTarget {
  Home = 'Home',
  About = 'About',
  Contact = 'Contact Us'
}

export const navigationItems: Ref<NavigationItem[]> = ref<NavigationItem[]>([
  {
    title: NavigationTarget.Home,
    isActive: true,
    path: '/'
  },
  {
    title: NavigationTarget.About,
    isActive: false,
    path: '/about'
  },
  {
    title: NavigationTarget.Contact,
    isActive: false,
    path: '/contact'
  }
])
