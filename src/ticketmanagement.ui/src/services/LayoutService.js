import axios from "axios";
import AuthService from "./AuthService";

const venueApi = 'https://localhost:7162';

class LayoutService {
  async getById(id)
  {
    const response = await axios.get(venueApi + `/layouts/${id}`, {
        headers: {
          'Authorization': AuthService.getTokenHeader()
        }
    });
    return response.data;
  }
}
export default new LayoutService();