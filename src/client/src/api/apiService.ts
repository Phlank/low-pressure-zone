import axios from 'axios'
import process from 'node:process'

const axiosInstance = axios.create({
  baseURL: process.env.BASE_URL,
  timeout: 1000
})

export const users = {
  get: axiosInstance.get<User[]>()
}
