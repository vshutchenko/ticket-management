import { useState, React } from "react";
import { useNavigate } from "react-router";
import { Link } from "react-router-dom";
import AuthService from '../../services/AuthService';
import { useTranslation } from 'react-i18next';

import {
  Collapse,
  Navbar,
  NavbarToggler,
  NavbarBrand,
  Nav,
  NavItem,
  NavLink,
  UncontrolledDropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem
} from 'reactstrap';

export default function Layout() {
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [isOpen, setIsOpen] = useState(false);

  function logout() {
    AuthService.logout();
    navigate("/");
  }

  return (
    <div>
      <Navbar color="light" light expand="md">
        <NavbarBrand href="/">Ticket management</NavbarBrand>
        <NavbarToggler onClick={() => setIsOpen(!isOpen)} />
        <Collapse isOpen={isOpen} navbar>
          <Nav className="ml-auto" navbar>
            <NavItem>
              <NavLink href="/" className="text-dark">{t("Events")}</NavLink>
            </NavItem>
            {AuthService.isEventManager() && (
              <NavItem>
                <NavLink href="/Event/NotPublishedEvents">{t("Not published events")}</NavLink>
              </NavItem>)
            }
            {AuthService.isVenueManager() && (
              <NavItem>
                <NavLink href="/Venue/VenueList">{t("Venue management")}</NavLink>
              </NavItem>)
            }
            {AuthService.isAuthenticated() ? (
              <UncontrolledDropdown nav inNavbar>
                <DropdownToggle nav caret>
                  {AuthService.getCurrentUser().email}
                </DropdownToggle>
                <DropdownMenu end>
                  <DropdownItem>
                    <NavLink href="/">{t("Add funds")}</NavLink>
                  </DropdownItem>
                  <DropdownItem>
                    <NavLink href={`/Account/EditUser?id=${AuthService.getCurrentUser().id}`}>{t("Edit profile")}</NavLink>
                  </DropdownItem>
                  <DropdownItem>
                    <NavLink href="/">{t("Change password")}</NavLink>
                  </DropdownItem>
                  <DropdownItem divider />
                  <DropdownItem>
                    <NavLink href="/" onClick={() => logout()}>{t("Logout")}</NavLink>
                  </DropdownItem>
                </DropdownMenu>
              </UncontrolledDropdown>) : (
              <>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Account/Login">{t("Login")}</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/Account/Register">{t("Register")}</NavLink>
                </NavItem>
              </>)
            }
          </Nav>
        </Collapse>
      </Navbar>
    </div>
  );
}