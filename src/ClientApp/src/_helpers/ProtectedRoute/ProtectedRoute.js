import React from 'react';
import { Route, Redirect } from 'react-router-dom';
import Login from '../../components/Login/Login';
import {useSelector} from 'react-redux';

function ProtectedRoute(props) {
  const cookieSet = useSelector((store) => store.login.loggedIn);
  const loggedIn = localStorage.getItem('user');
  const {
    authRedirect,
    ...otherProps
  } = props;
  const ComponentToProtect = props.component || (() => props.children);

  let ComponentToShow;

  if (loggedIn && cookieSet) {
    ComponentToShow = ComponentToProtect;
  } else {
    ComponentToShow = Login;
  }

  if (loggedIn && cookieSet && authRedirect != null) {
    return <Redirect exact from={otherProps.path} to={authRedirect} />;
  } else if (!loggedIn && !cookieSet && authRedirect != null) {
    ComponentToShow = ComponentToProtect;
  }

  return (
    <Route
      {...otherProps}
    >
      <ComponentToShow />
    </Route>
  );
}

export default ProtectedRoute;

/* import React from 'react';
import { Route, Redirect } from 'react-router-dom';

function ProtectedRoute({ component: Component, roles, ...rest }) {
    return (
        <Route {...rest} render={props => {
            if (!localStorage.getItem('user')) {
                // not logged in so redirect to login page with the return url
                return <Redirect to={{ pathname: '/login', state: { from: props.location } }} />
            }

            // logged in so return component
            return <Component {...props} />
        }} />
    );
}

export default ProtectedRoute; */
