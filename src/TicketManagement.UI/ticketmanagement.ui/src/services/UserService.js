import axios from "axios";
import AuthService from "./AuthService";

const userApi = process.env.REACT_APP_USER_API;

class UserService {
  async getById(id) {
    const response = await axios.get(userApi + `/users/${id}`, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
    return response.data;
  }

  async update(user) {
    await axios.put(userApi + '/users', user, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    }).then(response => {
      const token = response.data;
      AuthService.refreshToken(token)
    });
  }

  async changePassword(id, currentPassword, newPasword) {
    let data = {
      currentPassword: currentPassword,
      newPassword: newPasword
    }

    await axios.put(userApi + `/users/${id}/password`, data, {
      headers: {
        'Authorization': `Bearer ${AuthService.getToken()}`
      }
    });
  }
}
export default new UserService();