import axios from "axios";
import jwtDecode from "jwt-decode";
import i18n from 'i18next';

const userApi = process.env.REACT_APP_USER_API;

class AuthService {
  async login(email, password) {
    await axios
      .post(userApi + '/users/login', {
        email,
        password
      })
      .then(response => {
        const token = response.data;
        localStorage.setItem('jwt', token);
        const user = jwtDecode(token);
        const lang = user.culture.slice(0, 2);

        i18n.changeLanguage(lang);
      });
  }

  logout() {
    localStorage.removeItem('jwt');
    i18n.changeLanguage('en');
  }

  refreshToken(token) {

    localStorage.setItem('jwt', token);
    const user = jwtDecode(token);
    const lang = user.culture.slice(0, 2);
    i18n.changeLanguage(lang);
  }

  async register(user) {
    await axios.post(userApi + '/users/register', user).then(response => {
      const token = response.data;
      localStorage.setItem('jwt', token);
      const user = jwtDecode(token);
      const lang = user.culture.slice(0, 2);

      i18n.changeLanguage(lang);
    });
  }

  getCurrentUser() {
    const token = localStorage.getItem('jwt');
    return token ? jwtDecode(token) : null;
  }

  getToken() {
    const token = localStorage.getItem('jwt');
    return token;
  }

  isAuthenticated() {
    const token = localStorage.getItem('jwt');

    if (token) {
      return true;
    }

    return false;
  }

  isUser() {
    const token = localStorage.getItem('jwt');

    if (token) {
      const user = jwtDecode(token);
      return user.role.includes('User');
    }

    return false;
  }

  isEventManager() {
    const token = localStorage.getItem('jwt');

    if (token) {
      const user = jwtDecode(token);
      return user.role.includes('Event manager');
    }

    return false;
  }

  isVenueManager() {
    const token = localStorage.getItem('jwt');

    if (token) {
      const user = jwtDecode(token);
      return user.role.includes('Venue manager');
    }

    return false;
  }
}
export default new AuthService();